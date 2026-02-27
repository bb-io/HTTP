using Apps.HTTP.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
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

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        RestResponse restResponse = await ExecuteAsync(request);
        if (!restResponse.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(restResponse);
        }

        return restResponse;
    }

    public async Task<Stream> GetFileStreamAsync(RestRequest request)
    {
        var stream = await this.DownloadStreamAsync(request);

        if (stream == null)
        {
            throw new PluginApplicationException("Could not retrieve file stream from the source.");
        }

        return stream;
    }

    public async Task<RestResponse> GetFileMetadataAsync(RestRequest request)
    {
        return await this.ExecuteAsync(request);
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        return new PluginApplicationException(response.ErrorMessage ?? response.Content ?? $"An error occurred. Status code: {response.StatusCode}");
    }
}