using System.Collections.Generic;
using Newtonsoft.Json;

namespace MAVN.Service.PayrexxIntegration.Client.Models.Responses
{
    public class PaymentGatewayResponse : ResponseBase<PaymentGatewayData>
    {
    }

    public class PaymentGatewayData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("psp")]
        public List<int> PaymentProviders { get; set; }
    }
}
