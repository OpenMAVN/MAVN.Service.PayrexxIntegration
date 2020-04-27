using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.PayrexxIntegration.Client;
using MAVN.Service.PayrexxIntegration.Client.Models.Requests;
using MAVN.Service.PaymentIntegrationPlugin.Client;
using MAVN.Service.PaymentIntegrationPlugin.Client.Models.Responses;
using MAVN.Service.PaymentIntegrationPlugin.Client.Models.Requests;
using MAVN.Service.PayrexxIntegration.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.PayrexxIntegration.Controllers
{
    [Route("api/payment")]
    public class PayrexxIntegrationController : Controller, IPaymentIntegrationPluginApi
    {
        private readonly IPartnerIntegrationPropertiesFetcherService _partnerIntegrationPropertiesFetcherService;
        private readonly ILog _log;

        public PayrexxIntegrationController(
            IPartnerIntegrationPropertiesFetcherService partnerIntegrationPropertiesFetcherService,
            ILogFactory logFactory)
        {
            _partnerIntegrationPropertiesFetcherService = partnerIntegrationPropertiesFetcherService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Get a list of payment integration properties
        /// </summary>
        [HttpGet("requirements")]
        public Task<PaymentIntegrationPropertiesResponse> GetPaymentIntegrationPropertiesAsync()
        {
            return Task.FromResult(
                new PaymentIntegrationPropertiesResponse
                {
                    Properties = new List<PaymentIntegrationProperty>
                    {
                        new PaymentIntegrationProperty
                        {
                            Name = "Instance name",
                            Description = "Payrexx instance name",
                        },
                        new PaymentIntegrationProperty
                        {
                            Name = "API Secret",
                            Description = "Payrexx api key",
                            IsSecret = true,
                        },
                    }
                });
        }

        /// <summary>
        /// Get a list of supported currencies
        /// </summary>
        [HttpGet("currencies")]
        public Task<List<string>> GetPaymentIntegrationSupportedCurrenciesAsync()
        {
            return Task.FromResult(new List<string> { "CHF" });
        }

        /// <summary>
        /// Checks configuration of payment integration for partner
        /// </summary>
        /// <param name="request">Check payment integration request</param>
        [HttpPost("check")]
        public async Task<bool> CheckPaymentIntegrationAsync(CheckPaymentIntegrationRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var res = await client.Api.CheckSignatureAsync();
                return res.Status == "success";
            }
            catch (Exception e)
            {
                _log.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Generates a payment from integrated payment provider.
        /// </summary>
        /// <param name="request">Payment generation request</param>
        [HttpPost]
        public async Task<PaymentResponse> GeneratePaymentAsync(GeneratePaymentRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var res = await client.Api.CreatePaymentGatewayAsync(
                new PaymentGatewayRequest
                {
                    Amount = request.Amount,
                    Currency = request.Currency,
                    SuccessRedirectUrl = request.SuccessRedirectUrl,
                    FailedRedirectUrl = request.FailRedirectUrl,
                    ReferenceId = request.PaymentRequestId,
                });

                var payment = res.Data[0];

                return new PaymentResponse
                {
                    PaymentId = payment.Id.ToString(),
                    PaymentPageUrl = payment.Link,
                };
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }

        /// <summary>
        /// Checks for a payment status
        /// </summary>
        /// <param name="request">Check payment request</param>
        [HttpGet]
        public async Task<PaymentStatus> CheckPaymentAsync(CheckPaymentRequest request)
        {
            var integrationProperties = await _partnerIntegrationPropertiesFetcherService.FetchPropertiesAsync(request.PartnerId);

            var client = new PayrexxIntegrationClient(
                integrationProperties.ApiBaseUrl,
                integrationProperties.InstanceName,
                integrationProperties.ApiKey);

            try
            {
                var paymentStatus = await client.Api.GetPaymentGatewayAsync(int.Parse(request.PaymentId));

                if (paymentStatus.Status != "success")
                    return PaymentStatus.NotFound;

                switch(paymentStatus.Data[0].Status)
                {
                    case "waiting":
                        return PaymentStatus.Pending;
                    case "confirmed":
                        return PaymentStatus.Success;
                    case "authorized":
                    case "reserved":
                        return PaymentStatus.Processing;
                    default:
                        throw new NotSupportedException($"Payment status {paymentStatus.Data[0].Status} is not supported");
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }
    }
}
