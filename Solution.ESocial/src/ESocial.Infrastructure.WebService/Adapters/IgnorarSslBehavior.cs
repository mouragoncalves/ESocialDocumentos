using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ESocial.Infrastructure.WebService.Adapters;

/// <summary>
/// Behavior que desabilita a validação SSL do servidor no transporte HTTP do WCF.
/// Necessário em ambientes de desenvolvimento onde os certificados ICP-Brasil
/// não estão instalados no sistema operacional.
/// </summary>
internal sealed class IgnorarSslBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        // System.ServiceModel.Http 10.x expects Func<HttpClientHandler, HttpMessageHandler>
        bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(handler =>
        {
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            return handler;
        }));
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
    public void Validate(ServiceEndpoint endpoint) { }
}
