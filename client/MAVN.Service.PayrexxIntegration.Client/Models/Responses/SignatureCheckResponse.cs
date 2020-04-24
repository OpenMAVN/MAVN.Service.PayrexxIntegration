using Newtonsoft.Json;

namespace MAVN.Service.PayrexxIntegration.Client.Models.Responses
{
    public class SignatureCheckResponse : ResponseBase<SignatureCheckData>
    {
    }

    public class SignatureCheckData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
