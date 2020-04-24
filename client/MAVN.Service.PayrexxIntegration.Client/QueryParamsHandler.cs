using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAVN.Service.PayrexxIntegration.Client
{
    internal class QueryParamsHandler : DelegatingHandler
    {
        private const string InstanceQueryParamName = "instance";
        private const string SignatureQueryParamName = "ApiSignature";

        private readonly byte[] _apiKeyBytes;
        private readonly string _instance;

        public QueryParamsHandler(string instance, string apiKey)
        {
            _instance = instance;
            _apiKeyBytes = new UTF8Encoding().GetBytes(apiKey);
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[InstanceQueryParamName] = _instance;

            if (request.Method == HttpMethod.Get)
            {
                query[SignatureQueryParamName] = GenerateSignature(request.RequestUri.Query);
            }
            else
            {
                var json = await request.Content.ReadAsStringAsync();
                var jObj = (JObject)JsonConvert.DeserializeObject(json);
                var queryStr = string.Join("&",
                    jObj.Children()
                        .Cast<JProperty>()
                        .Select(jp => jp.Name + "=" + HttpUtility.UrlEncode(jp.Value.ToString().Replace(" ", "%20"))));
                var signature = GenerateSignature(queryStr);
                queryStr += $"&{SignatureQueryParamName}={signature}";
                request.Content = new StringContent(queryStr);
            }
            uriBuilder.Query = query.ToString();

            request.RequestUri = uriBuilder.Uri;

            var result = await base.SendAsync(request, cancellationToken);
            return result;

            //return base.SendAsync(request, cancellationToken);
        }

        private string GenerateSignature(string queryString)
        {
            byte[] messageBytes = new UTF8Encoding().GetBytes(queryString);
            byte[] hashmessage = new HMACSHA256(_apiKeyBytes).ComputeHash(messageBytes);
            var signature = Convert.ToBase64String(hashmessage);
            return HttpUtility.UrlEncode(signature);
        }
    }
}
