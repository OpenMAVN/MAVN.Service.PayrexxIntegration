using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MAVN.Service.PayrexxIntegration.Client.Models.Responses
{
    /// <summary>
    /// Base response class
    /// </summary>
    public abstract class ResponseBase<T>
    {
        /// <summary>Response status</summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>Response data</summary>
        [JsonProperty("data")]
        public List<T> Data { get; set; }
    }
}
