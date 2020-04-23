using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace MAVN.Service.PayrexxIntegration.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public PayrexxIntegrationSettings PayrexxIntegrationService { get; set; }
    }
}
