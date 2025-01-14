using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.HTTP;

public class HttpClient : BlackBirdRestClient
{
    public HttpClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
        : base(new RestClientOptions { ThrowOnAnyError = false, BaseUrl = GetBaseUrl(authenticationCredentialsProviders) }) 
    { }

    private static Uri GetBaseUrl(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var baseUrl = authenticationCredentialsProviders.First(p => p.KeyName == "Base URL").Value;
        return new(baseUrl);
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
           return new PluginApplicationException();
    }
}