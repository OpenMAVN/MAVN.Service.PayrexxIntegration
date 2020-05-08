using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Newtonsoft.Json;

namespace MAVN.Service.PayrexxIntegration.Client.Models.Requests
{
    public class PaymentGatewayRequest
    {
        private decimal _amount;
        private string _successRedirectUrl;
        private string _failedRedirectUrl;
        private string _cancelRedirectUrl;

        [JsonProperty("amount")]
        [Required]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = decimal.Truncate(value * 100); }
        }

        [JsonProperty("currency")]
        [Required]
        public string Currency { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("referenceId")]
        [Required]
        public string ReferenceId { get; set; }

        [JsonProperty("successRedirectUrl")]
        public string SuccessRedirectUrl
        {
            get { return _successRedirectUrl; }
            set
            {
                _successRedirectUrl = HttpUtility.UrlEncode(value);
                if (_failedRedirectUrl == null)
                {
                    _failedRedirectUrl = _successRedirectUrl;
                }
            }
        }

        [JsonProperty("failedRedirectUrl")]
        public string FailedRedirectUrl
        {
            get { return _failedRedirectUrl; }
            set
            {
                _failedRedirectUrl = HttpUtility.UrlEncode(value);
                if (_cancelRedirectUrl == null)
                    _cancelRedirectUrl = _failedRedirectUrl;
            }
        }

        [JsonProperty("cancelRedirectUrl")]
        public string CancelRedirectUrl
        {
            get { return _cancelRedirectUrl; }
            set
            {
                _cancelRedirectUrl = HttpUtility.UrlEncode(value);
                if (_failedRedirectUrl == null)
                    _failedRedirectUrl = _cancelRedirectUrl;
            }
        }

        [JsonProperty("psp")]
        public List<int> PaymentProviders { get; set; }

        [JsonProperty("skipResultPage")]
        public bool SkipResultPage { get; set; }
    }
}
