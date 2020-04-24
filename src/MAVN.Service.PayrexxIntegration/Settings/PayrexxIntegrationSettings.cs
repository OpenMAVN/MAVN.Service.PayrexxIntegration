using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.PayrexxIntegration.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PayrexxIntegrationSettings
    {
        public DbSettings Db { get; set; }

        public string PayrexxApiBaseUrl { get; set; }
    }
}
