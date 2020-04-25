using System;
using System.Threading.Tasks;
using MAVN.Service.PayrexxIntegration.Domain;
using MAVN.Service.PayrexxIntegration.Domain.Services;

namespace MAVN.Service.PayrexxIntegration.DomainServices
{
    public class PartnerIntegrationPropertiesFetcherService : IPartnerIntegrationPropertiesFetcherService
    {
        private readonly string _apiBaseUrl;

        public PartnerIntegrationPropertiesFetcherService(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;
        }

        public Task<PartnerIntegrationProperties> FetchPropertiesAsync(Guid partnerId)
        {
            throw new NotSupportedException();

            //var partnerIntegrationProperties = await _partnerManagementServiceClient.

            //return new PartnerIntegrationProperties
            //{
            //    ApiBaseUrl = _apiBaseUrl,
            //    InstanceName = ,
            //    ApiKey = ,
            //};
        }
    }
}
