using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public class PutRequest : Request
{
    private readonly string _body;

    [Display("Is request body in JSON format")]
    public bool IsBodyInJsonFormat { get; init; }
    
    [Display("Request body")]
    public string Body { get => _body; init => _body = ReplaceBrackets(value)!; }
}