using Apps.HTTP.Models.Requests;
using Apps.HTTP.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HTTP;

[ActionList]
public class Actions : BaseInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }
    
    [Action("Get", Description = "Perform a GET request to the specified endpoint.")]
    public async Task<ResponseDto> Get([ActionParameter] GetRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        CheckIfValidJson(input.QueryParameters, "Query parameters");

        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        if (input.QueryParameters != null)
        { 
            var queryParameters = ConvertToDictionary<string>(input.QueryParameters);
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParameters);
        }

        var request = new HttpRequest(endpoint, Method.Get, Creds);
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }

        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    [Action("Get file", Description = "Perform a GET request to the specified endpoint to download a file.")]
    public async Task<FileResponseDto> GetFile([ActionParameter] GetRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        CheckIfValidJson(input.QueryParameters, "Query parameters");

        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        if (input.QueryParameters != null)
        { 
            var queryParameters = ConvertToDictionary<string>(input.QueryParameters);
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParameters);
        }

        var request = new HttpRequest(endpoint, Method.Get, Creds);
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }

        var response = await client.ExecuteWithErrorHandling(request);
        return new FileResponseDto(response, _fileManagementClient);
    }
    
    [Action("Post", Description = "Perform a POST request to the specified endpoint.")]
    public async Task<ResponseDto> Post([ActionParameter] PostRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        if (input.IsBodyInJsonFormat) 
            CheckIfValidJson(input.Body, "Request body");
        
        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        var request = new HttpRequest(endpoint, Method.Post, Creds);
        request.AddJsonBody(input.Body);
        
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }
    
        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    [Action("Put", Description = "Perform a PUT request to the specified endpoint.")]
    public async Task<ResponseDto> Put([ActionParameter] PutRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        CheckIfValidJson(input.QueryParameters, "Query parameters");
        if (input.IsBodyInJsonFormat) 
            CheckIfValidJson(input.Body, "Request body");
        
        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        if (input.QueryParameters != null)
        { 
            var queryParameters = ConvertToDictionary<string>(input.QueryParameters);
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParameters);
        }
        
        var request = new HttpRequest(endpoint, Method.Put, Creds);
        request.AddJsonBody(input.Body);
        
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }
    
        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    [Action("Patch", Description = "Perform a PATCH request to the specified endpoint.")]
    public async Task<ResponseDto> Patch([ActionParameter] PatchRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        CheckIfValidJson(input.QueryParameters, "Query parameters");
        if (input.IsBodyInJsonFormat) 
            CheckIfValidJson(input.Body, "Request body");
        
        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        if (input.QueryParameters != null)
        { 
            var queryParameters = ConvertToDictionary<string>(input.QueryParameters);
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParameters);
        }
        
        var request = new HttpRequest(endpoint, Method.Patch, Creds);
        request.AddJsonBody(input.Body);
        
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }

        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    [Action("Delete", Description = "Perform a DELETE request to the specified endpoint.")]
    public async Task<ResponseDto> Delete([ActionParameter] DeleteRequest input)
    {
        CheckIfValidJson(input.Headers, "Headers");
        CheckIfValidJson(input.QueryParameters, "Query parameters");

        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        if (input.QueryParameters != null)
        { 
            var queryParameters = ConvertToDictionary<string>(input.QueryParameters);
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParameters);
        }
        
        var request = new HttpRequest(endpoint, Method.Delete, Creds);
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }
    
        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    private static Dictionary<string, TValue> ConvertToDictionary<TValue>(string json) 
        => JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
    
    private static void CheckIfValidJson(string? json, string parameterName)
    {
        if (json == null)
            return; 
        
        try
        {
            JObject.Parse(json);
        }
        catch (JsonReaderException)
        {
            throw new PluginMisconfigurationException($"{parameterName} must be in JSON format. Example of valid JSON: " + 
                                "{ \"key\": \"value\", \"key2\": \"value2\" }");
        }
    }
}