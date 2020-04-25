using System;
using System.Threading.Tasks;

namespace MAVN.Service.PayrexxIntegration.Domain.Services
{
    public interface IPartnerIntegrationPropertiesFetcherService
    {
        Task<PartnerIntegrationProperties> FetchPropertiesAsync(Guid partnerId);
    }
}
