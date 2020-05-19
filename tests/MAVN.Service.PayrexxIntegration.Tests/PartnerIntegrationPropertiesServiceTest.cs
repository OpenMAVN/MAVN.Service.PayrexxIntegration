using System;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.PayrexxIntegration.Domain.Enums;
using MAVN.Service.PayrexxIntegration.DomainServices;
using Moq;
using Xunit;

namespace MAVN.Service.PayrexxIntegration.Tests
{
    public class PartnerIntegrationPropertiesServiceTest
    {

        private readonly Mock<ICustomerProfileClient> _customerProfileClient = new Mock<ICustomerProfileClient>();
        private readonly string _fakeApiBaseUrl = Guid.NewGuid().ToString();

        [Fact]
        public async void CheckIntegrationSupportedCurrency_ReturnsNotEmptyStringList()
        {
            var expectedCount = Enum.GetNames(typeof(IntegrationSupportedCurrency)).Length;

            var result = await CreateSutInstance().GetIntegrationCurrency();

            Assert.Equal(expectedCount, result.Count);
        }

        private PartnerIntegrationPropertiesService CreateSutInstance()
        {
            return new PartnerIntegrationPropertiesService(_customerProfileClient.Object, _fakeApiBaseUrl);
        }
    }
}
