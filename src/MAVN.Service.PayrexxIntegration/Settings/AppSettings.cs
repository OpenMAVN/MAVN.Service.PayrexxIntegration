using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.CustomerProfile.Client;

namespace MAVN.Service.PayrexxIntegration.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public PayrexxIntegrationSettings PayrexxIntegrationService { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
    }
}
