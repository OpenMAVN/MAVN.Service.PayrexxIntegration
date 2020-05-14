using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.PayrexxIntegration.Domain;
using MAVN.Service.PayrexxIntegration.Domain.Enums;
using MAVN.Service.PayrexxIntegration.Domain.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAVN.Service.PayrexxIntegration.DomainServices
{
    public class PartnerIntegrationPropertiesService : IPartnerIntegrationPropertiesService
    {
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly string _apiBaseUrl;

        public PartnerIntegrationPropertiesService(
            ICustomerProfileClient customerProfileClient,
            string apiBaseUrl)
        {
            _customerProfileClient = customerProfileClient;
            _apiBaseUrl = apiBaseUrl;
        }

        public List<IntegrationProperty> GetIntegrationProperties()
        {
            return new List<IntegrationProperty>
            {
                new IntegrationProperty
                {
                    Name = "Instance name",
                    Description = "Payrexx instance name",
                    JsonKey = Constants.InstanceJsonProperty,
                },
                new IntegrationProperty
                {
                    Name = "API Secret",
                    Description = "Payrexx api key",
                    JsonKey = Constants.ApiKeyJsonProperty,
                    IsSecret = true,
                },
            };
        }

        public async Task<PayrexxIntegrationProperties> FetchPropertiesAsync(Guid partnerId)
        {
            var partnerIntegrationProperties = await _customerProfileClient.PaymentProviderDetails.GetByPartnerIdAndPaymentProviderAsync(
                partnerId, Constants.PayrexxProvider);

            if (partnerIntegrationProperties.ErrorCode != PaymentProviderDetailsErrorCodes.None)
                return new PayrexxIntegrationProperties { ErrorCode = IntegrationPropertiesErrorCode.PartnerConfigurationNotFound };

            return DeserializePayrexxIntegrationProperties(partnerIntegrationProperties.PaymentProviderDetails.PaymentIntegrationProperties);
        }

        public PayrexxIntegrationProperties DeserializePayrexxIntegrationProperties(string paymentIntegrationProperties)
        {
            JObject jobj;
            try
            {
                jobj = (JObject)JsonConvert.DeserializeObject(paymentIntegrationProperties);
            }
            catch
            {
                return new PayrexxIntegrationProperties
                {
                    ErrorCode = IntegrationPropertiesErrorCode.Fail
                };
            }

            var instance = jobj[Constants.InstanceJsonProperty]?.ToString();

            if (string.IsNullOrWhiteSpace(instance))
            {
                return new PayrexxIntegrationProperties
                {
                    ErrorCode = IntegrationPropertiesErrorCode.PartnerConfigurationPropertyIsMissing
                };
            }

            var apiKey = jobj[Constants.ApiKeyJsonProperty]?.ToString();

            return new PayrexxIntegrationProperties
            {
                ApiBaseUrl = _apiBaseUrl,
                InstanceName = instance,
                ApiKey = apiKey,
                ErrorCode = IntegrationPropertiesErrorCode.None,
            };
        }
    }
}
