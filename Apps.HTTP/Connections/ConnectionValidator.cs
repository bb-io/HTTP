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

        var raw = creds.Get(CredNames.BaseUrl).Value?.Trim();

        if (string.IsNullOrWhiteSpace(raw))
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = "Base URL is empty"
            };
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var host = new UriBuilder(raw.Contains("://") ? raw : $"http://{raw}").Host;

            if (string.IsNullOrWhiteSpace(host))
            {
                return new ConnectionValidationResponse
                {
                    IsValid = false,
                    Message = "Invalid URL"
                };
            }

            var addresses = await Dns.GetHostAddressesAsync(host);

            return new ConnectionValidationResponse
            {
                IsValid = addresses.Length > 0,
                Message = addresses.Length > 0 ? "Success" : "Ping failed"
            };
        }
        catch (UriFormatException)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = "Invalid URL"
            };
        }
        catch (SocketException)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = "Ping failed. No such host is known"
            };
        }
    }
}