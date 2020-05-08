using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.PayrexxIntegration.Client;
using MAVN.Service.PayrexxIntegration.Client.Models.Requests;
using MAVN.Service.PaymentIntegrationPlugin.Client;
using MAVN.Service.PaymentIntegrationPlugin.Client.Models.Responses;
using MAVN.Service.PaymentIntegrationPlugin.Client.Models.Requests;
using MAVN.Service.PayrexxIntegration.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using MAVN.Service.PayrexxIntegration.Domain;
using System.Net;
using MAVN.Service.PayrexxIntegration.Domain.Enums;
using Common;

namespace MAVN.Service.PayrexxIntegration.Controllers
{
    [Route("api/payment")]
    public class PayrexxIntegrationController : Controller, IPaymentIntegrationPluginApi
    {
        private readonly IPartnerIntegrationPropertiesService _partnerIntegrationPropertiesFetcherService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public PayrexxIntegrationController(
            IPartnerIntegrationPropertiesService partnerIntegrationPropertiesFetcherService,
            IMapper mapper,
            ILogFactory logFactory)
        {
            _partnerIntegrationPropertiesFetcherService = partnerIntegrationPropertiesFetcherService;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Get a list of payment integration properties
        /// </summary>
        [HttpGet("requirements")]
        [ProducesResponseType(typeof(PaymentIntegrationPropertiesResponse), (int)HttpStatusCode.OK)]
        public Task<PaymentIntegrationPropertiesResponse> GetPaymentIntegrationPropertiesAsync()
        {
            var properties = _partnerIntegrationPropertiesFetcherService.GetIntegrationProperties();

            return Task.FromResult(
                new PaymentIntegrationPropertiesResponse
                {
                    PaymentProvider = Constants.PayrexxProvider,
                    Properties = _mapper.Map<List<PaymentIntegrationProperty>>(properties),
                });
        }

        /// <summary>
        /// Get a list of supported currencies
        /// </summary>
        [HttpGet("currencies")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public Task<List<string>> GetPaymentIntegrationSupportedCurrenciesAsync()
        {
            return Task.FromResult(new List<string> { "CHF" });
        }

        /// <summary>
        /// Checks configuration of payment integration for partner
        /// </summary>
        /// <param name="request">Check payment integration request</param>
        [HttpPost("check")]
        [ProducesResponseType(typeof(CheckIntegrationErrorCode), (int)HttpStatusCode.OK)]
        public async Task<CheckIntegrationErrorCode> CheckPaymentIntegrationAsync([FromBody] CheckPaymentIntegrationRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);
            if (integrationProperties.ErrorCode != IntegrationPropertiesErrorCode.None)
                return _mapper.Map<CheckIntegrationErrorCode>(integrationProperties.ErrorCode);

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var res = await client.Api.CheckSignatureAsync();
                return res.Status == "success"
                    ? CheckIntegrationErrorCode.None
                    : CheckIntegrationErrorCode.Fail;
            }
            catch (Exception e)
            {
                _log.Warning(null, exception: e);
                return CheckIntegrationErrorCode.Fail;
            }
        }

        /// <summary>
        /// Generates a payment from integrated payment provider.
        /// </summary>
        /// <param name="request">Payment generation request</param>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponse), (int)HttpStatusCode.OK)]
        public async Task<PaymentResponse> GeneratePaymentAsync([FromBody] GeneratePaymentRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);
            if (integrationProperties.ErrorCode != IntegrationPropertiesErrorCode.None)
                return new PaymentResponse
                {
                    ErrorCode = _mapper.Map<CheckIntegrationErrorCode>(integrationProperties.ErrorCode),
                };

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var paymentGatewayRequest = new PaymentGatewayRequest
                {
                    Amount = request.Amount,
                    Currency = request.Currency,
                    SuccessRedirectUrl = request.SuccessRedirectUrl,
                    FailedRedirectUrl = request.FailRedirectUrl,
                    ReferenceId = request.PaymentRequestId,
                    SkipResultPage = true
                };

                var res = await client.Api.CreatePaymentGatewayAsync(paymentGatewayRequest);

                _log.Info("Call CreatePaymentGatewayAsync with data: " + paymentGatewayRequest.ToJson());

                var payment = res.Data[0];

                return new PaymentResponse
                {
                    PaymentId = payment.Id.ToString(),
                    PaymentPageUrl = payment.Link,
                };
            }
            catch (Exception e)
            {
                _log.Warning(null, exception: e);
                return new PaymentResponse
                {
                    ErrorCode = CheckIntegrationErrorCode.Fail,
                };
            }
        }

        /// <summary>
        /// Checks for a payment status
        /// </summary>
        /// <param name="request">Check payment request</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaymentStatusResponse), (int)HttpStatusCode.OK)]
        public async Task<PaymentStatusResponse> CheckPaymentAsync(CheckPaymentRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);
            if (integrationProperties.ErrorCode != IntegrationPropertiesErrorCode.None)
                return new PaymentStatusResponse
                {
                    ErrorCode = _mapper.Map<CheckIntegrationErrorCode>(integrationProperties.ErrorCode),
                };

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var paymentStatus = await client.Api.GetPaymentGatewayAsync(int.Parse(request.PaymentId));

                var result = new PaymentStatusResponse { ErrorCode = CheckIntegrationErrorCode.None };
                if (paymentStatus.Status != "success")
                {
                    result.PaymentStatus = PaymentStatus.NotFound;
                    return result;
                }

                switch(paymentStatus.Data[0].Status)
                {
                    case "waiting":
                        result.PaymentStatus = PaymentStatus.Pending;
                        break;
                    case "confirmed":
                        result.PaymentStatus = PaymentStatus.Success;
                        break;
                    case "authorized":
                    case "reserved":
                        result.PaymentStatus = PaymentStatus.Processing;
                        break;
                    default:
                        throw new NotSupportedException($"Payment status {paymentStatus.Data[0].Status} is not supported");
                }
                return result;
            }
            catch (Exception e)
            {
                _log.Warning(null, exception: e);
                return new PaymentStatusResponse
                {
                    ErrorCode = CheckIntegrationErrorCode.Fail,
                };
            }
        }
    }
}
