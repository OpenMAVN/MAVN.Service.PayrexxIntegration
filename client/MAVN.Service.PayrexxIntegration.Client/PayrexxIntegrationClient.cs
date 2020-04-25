using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Infrastructure;

namespace MAVN.Service.PayrexxIntegration.Client
{
    /// <summary>
    /// PayrexxIntegration API aggregating interface.
    /// </summary>
    public class PayrexxIntegrationClient
    {
        /// <summary>Inerface to PayrexxIntegration Api.</summary>
        public IPayrexxIntegrationApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public PayrexxIntegrationClient(
            string payrexxBaseUrl,
            string instance,
            string apiKey)
        {
            var clientBuilder = HttpClientGenerator.BuildForUrl(payrexxBaseUrl)
                .WithAdditionalDelegatingHandler(new QueryParamsHandler(instance, apiKey))
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

            clientBuilder = clientBuilder.WithoutRetries();
            var httpClientGenerator = clientBuilder.Create();

            Api = httpClientGenerator.Generate<IPayrexxIntegrationApi>();
        }
    }
}
