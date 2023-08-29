using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public abstract class Request
{
    private readonly string? _headers;
    private readonly string? _queryParameters;
    
    public string Endpoint { get; init; }
    
    [Display("Headers in JSON format")]
    public string? Headers { get => _headers; init => _headers = ReplaceBrackets(value); }
    
    [Display("Query parameters in JSON format")]
    public string? QueryParameters { get => _queryParameters; init => _queryParameters = ReplaceBrackets(value)!; }

    protected static string? ReplaceBrackets(string? value) => value?.Replace("“", "\"").Replace("”", "\"");
}