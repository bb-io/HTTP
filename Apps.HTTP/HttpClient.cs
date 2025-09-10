using Apps.HTTP.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.HTTP;

public class HttpClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    : BlackBirdRestClient(new RestClientOptions
        { ThrowOnAnyError = false, BaseUrl = GetBaseUrl(authenticationCredentialsProviders) })
{
    private static Uri GetBaseUrl(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var baseUrl = authenticationCredentialsProviders.Get(CredNames.BaseUrl).Value;
        return new(baseUrl);
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        return new PluginApplicationException(response.ErrorMessage ?? response.Content ?? $"An error occurred. Status code: {response.StatusCode}");
    }
}