using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.PayrexxIntegration.Domain.Services;
using MAVN.Service.PayrexxIntegration.DomainServices;
using MAVN.Service.PayrexxIntegration.Services;
using MAVN.Service.PayrexxIntegration.Settings;

namespace MAVN.Service.PayrexxIntegration.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly AppSettings _settings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<PartnerIntegrationPropertiesService>()
                .As<IPartnerIntegrationPropertiesService>()
                .WithParameter(TypedParameter.From(_settings.PayrexxIntegrationService.PayrexxApiBaseUrl))
                .SingleInstance();

            builder.RegisterCustomerProfileClient(_settings.CustomerProfileServiceClient);
        }
    }
}
