using Lykke.HttpClientGenerator;

namespace MAVN.Service.PayrexxIntegration.Client
{
    /// <summary>
    /// PayrexxIntegration API aggregating interface.
    /// </summary>
    public class PayrexxIntegrationClient : IPayrexxIntegrationClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to PayrexxIntegration Api.</summary>
        public IPayrexxIntegrationApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public PayrexxIntegrationClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IPayrexxIntegrationApi>();
        }
    }
}
