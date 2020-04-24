using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.PayrexxIntegration.Client.Models.Requests;
using MAVN.Service.PayrexxIntegration.Client.Models.Responses;
using Refit;

namespace MAVN.Service.PayrexxIntegration.Client
{
    /// <summary>
    /// PayrexxIntegration client API interface.
    /// </summary>
    [PublicAPI]
    public interface IPayrexxIntegrationApi
    {
        /// <summary>Checks signature</summary>
        [Get("/v1.0/SignatureCheck/")]
        Task<SignatureCheckResponse> CheckSignatureAsync();

        /// <summary>Create payment gateway</summary>
        [Post("/v1.0/Gateway/")]
        Task<PaymentGatewayResponse> CreatePaymentGatewayAsync([Body] PaymentGatewayRequest request);

        /// <summary>Create payment gateway</summary>
        [Get("/v1.0/Gateway/{id}/")]
        Task<PaymentGatewayResponse> GetPaymentGatewayAsync(int id);
    }
}
