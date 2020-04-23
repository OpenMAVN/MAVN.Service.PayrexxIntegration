using JetBrains.Annotations;

namespace MAVN.Service.PayrexxIntegration.Client
{
    /// <summary>
    /// PayrexxIntegration client interface.
    /// </summary>
    [PublicAPI]
    public interface IPayrexxIntegrationClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - IPayrexxIntegrationApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        IPayrexxIntegrationApi Api { get; }
    }
}
