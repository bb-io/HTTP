using Apps.HTTP.Models.Requests;
using Apps.HTTP.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.HTTP;

[ActionList("HTTP actions")]
public class Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseInvocable(invocationContext)
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    [Action("Get", Description = "Perform a GET request to the specified endpoint.")]
    public async Task<ResponseDto> Get([ActionParameter] GetRequest input)
    {
        CheckIfValidHeaders(input.Headers);
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
        CheckIfValidHeaders(input.Headers);
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
        return new FileResponseDto(response, fileManagementClient);
    }
    
    [Action("Post", Description = "Perform a POST request to the specified endpoint.")]
    public async Task<ResponseDto> Post([ActionParameter] PostRequest input)
    {
        CheckIfValidHeaders(input.Headers);
        if (input.IsBodyInJsonFormat) 
            CheckIfValidJson(input.Body, "Request body");
        
        var client = new HttpClient(Creds);
        var endpoint = "/" + input.Endpoint.Trim('/');
        var request = new HttpRequest(endpoint, Method.Post, Creds).AddJsonBody(input.Body);
        
        if (input.Headers != null)
        {
            var headers = ConvertToDictionary<string>(input.Headers);
            request.AddHeaders(headers);
        }

        if (input.File != null)
        {
            var file = await fileManagementClient.DownloadAsync(input.File);
            var fileBytes = await file.GetByteData();
            request.AddFile(
                name: string.IsNullOrEmpty(input.FieldName) ? "file" : input.FieldName,
                bytes: fileBytes,
                fileName: input.File.Name,
                contentType: input.File.ContentType
            );
        }

        var response = await client.ExecuteWithErrorHandling(request);
        return new ResponseDto(response);
    }
    
    [Action("Put", Description = "Perform a PUT request to the specified endpoint.")]
    public async Task<ResponseDto> Put([ActionParameter] PutRequest input)
    {
        CheckIfValidHeaders(input.Headers);
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
        CheckIfValidHeaders(input.Headers);
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
        CheckIfValidHeaders(input.Headers);
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

        if (string.IsNullOrWhiteSpace(json))
            throw new PluginMisconfigurationException($"{parameterName} must be in JSON format. Example: {{ \"key\": \"value\" }}");

        EnsureNoUnescapedControlCharsInsideStrings(json, parameterName);

        try
        {
            using var sr = new StringReader(json);
            using var reader = new JsonTextReader(sr)
            {
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal,
                MaxDepth = 128,
                SupportMultipleContent = false,
            };

            var token = JToken.ReadFrom(reader);

            if (token.Type != JTokenType.Object)
                throw new PluginMisconfigurationException($"{parameterName} must be a JSON object. Example: {{ \"key\": \"value\" }}");

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.Comment)
                    throw new JsonReaderException("Comments are not allowed in JSON.");

                if (reader.TokenType != JsonToken.None)
                    throw new JsonReaderException("Additional content found after a valid JSON value.");
            }
        }
        catch (JsonReaderException ex)
        {
            throw new PluginMisconfigurationException(
                $"{parameterName} must be in JSON format. Error: {ex.Message}. Example: {{ \"key\": \"value\" }}");
        }
    }

    private static void EnsureNoUnescapedControlCharsInsideStrings(string json, string parameterName)
    {
        bool inString = false;
        bool escaped = false;

        for (int i = 0; i < json.Length; i++)
        {
            char c = json[i];

            if (!inString)
            {
                if (c == '"')
                    inString = true;

                continue;
            }

            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (c == '\\')
            {
                escaped = true;
                continue;
            }

            if (c == '"')
            {
                inString = false;
                continue;
            }

            if (c < 0x20)
            {
                var shown = c switch
                {
                    '\r' => "\\r",
                    '\n' => "\\n",
                    '\t' => "\\t",
                    _ => $"0x{(int)c:X2}"
                };

                throw new PluginMisconfigurationException(
                    $"{parameterName} must be valid JSON. Unescaped control character {shown} found inside a string value.");
            }
        }

        if (inString)
            throw new PluginMisconfigurationException($"{parameterName} must be valid JSON. Unterminated string detected.");
    }

    private static void CheckIfValidHeaders(string? headers)
    {
        if (string.IsNullOrWhiteSpace(headers))
            return;

        try
        {
            ConvertToDictionary<string>(json: headers);
        }
        catch (JsonException)
        {
            throw new PluginMisconfigurationException(
                "Invalid headers JSON. Please provide a flat JSON without nested objects"
            );
        }
    }
}