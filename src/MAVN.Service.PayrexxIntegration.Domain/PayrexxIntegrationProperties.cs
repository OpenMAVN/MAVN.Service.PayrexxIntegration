using MAVN.Service.PayrexxIntegration.Domain.Enums;

namespace MAVN.Service.PayrexxIntegration.Domain
{
    public class PayrexxIntegrationProperties
    {
        public IntegrationPropertiesErrorCode ErrorCode { get; set; }

        public string ApiBaseUrl { get; set; }

        public string InstanceName { get; set; }

        public string ApiKey { get; set; }
    }
}
