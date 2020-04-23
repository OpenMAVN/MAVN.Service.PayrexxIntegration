using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.PayrexxIntegration.Client 
{
    /// <summary>
    /// PayrexxIntegration client settings.
    /// </summary>
    public class PayrexxIntegrationServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
