using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.HTTP;

public class HttpRequest : BlackBirdRestRequest
{
    public HttpRequest(string endpoint, Method method,
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
        : base(endpoint, method, authenticationCredentialsProviders) { }

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds) { }
}