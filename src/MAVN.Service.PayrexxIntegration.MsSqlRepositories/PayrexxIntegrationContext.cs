using System.Data.Common;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.PayrexxIntegration.MsSqlRepositories
{
    public class PayrexxIntegrationContext : MsSqlContext
    {
        private const string Schema = ""; // TODO put proper schema name here

        // empty constructor needed for EF migrations
        [UsedImplicitly]
        public PayrexxIntegrationContext()
            : base(Schema)
        {
        }

        public PayrexxIntegrationContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        //Needed constructor for using InMemoryDatabase for tests
        public PayrexxIntegrationContext(DbContextOptions options)
            : base(Schema, options)
        {
        }

        public PayrexxIntegrationContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            // TODO put db entities models building code here
        }
    }
}
