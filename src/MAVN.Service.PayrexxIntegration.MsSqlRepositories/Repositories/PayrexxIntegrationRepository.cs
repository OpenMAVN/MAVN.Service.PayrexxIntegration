using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MAVN.Service.PayrexxIntegration.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.PayrexxIntegration.MsSqlRepositories.Repositories
{
    public class PayrexxIntegrationRepository : IPayrexxIntegrationRepository
    {
        private readonly IDbContextFactory<PayrexxIntegrationContext> _contextFactory;

        public PayrexxIntegrationRepository(IDbContextFactory<PayrexxIntegrationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
    }
}
