using Apps.HTTP.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.HTTP.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, 
        CancellationToken cancellationToken)
    {
        var credentialsProviders = authenticationCredentialsProviders as AuthenticationCredentialsProvider[] ?? authenticationCredentialsProviders.ToArray();
        
        var client = new HttpClient(credentialsProviders);
        
        var baseUrl = credentialsProviders.Get(CredNames.BaseUrl).Value;
        var leftPart = new Uri(baseUrl).GetLeftPart(UriPartial.Authority);
        var request = new HttpRequest(leftPart, Method.Get, credentialsProviders);

        try
        {
            await client.ExecuteWithErrorHandling(request);
            return new ConnectionValidationResponse
            {
                IsValid = true,
                Message = "Success"
            };
        }
        catch (Exception)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = "Ping failed"
            };
        }
    }
}