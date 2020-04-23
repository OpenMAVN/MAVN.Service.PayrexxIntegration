using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.SettingsReader;
using MAVN.Service.PayrexxIntegration.Domain.Repositories;
using MAVN.Service.PayrexxIntegration.MsSqlRepositories;
using MAVN.Service.PayrexxIntegration.MsSqlRepositories.Repositories;
using MAVN.Service.PayrexxIntegration.Settings;

namespace MAVN.Service.PayrexxIntegration.Modules
{
    [UsedImplicitly]
    public class DbModule : Module
    {
        private readonly string _connectionString;

        public DbModule(IReloadingManager<AppSettings> appSettings)
        {
            _connectionString = appSettings.CurrentValue.PayrexxIntegrationService.Db.SqlDbConnString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PayrexxIntegrationRepository>()
                .As<IPayrexxIntegrationRepository>()
                .SingleInstance();

            builder.RegisterMsSql(
                _connectionString,
                connString => new PayrexxIntegrationContext(connString, false),
                dbConn => new PayrexxIntegrationContext(dbConn));
        }
    }
}
