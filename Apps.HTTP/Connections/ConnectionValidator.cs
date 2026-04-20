using Apps.HTTP.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using System.Net;
using System.Net.Sockets;

namespace Apps.HTTP.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var creds = authenticationCredentialsProviders as AuthenticationCredentialsProvider[]
                    ?? authenticationCredentialsProviders.ToArray();

        try
        {
            var client = new HttpClient(creds);
            string host = client.Options.BaseUrl!.Host;

            var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
            return new ConnectionValidationResponse
            {
                IsValid = addresses.Length > 0,
                Message = addresses.Length > 0 ? "Success" : "Ping failed"
            };
        }
        catch (Exception ex)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}