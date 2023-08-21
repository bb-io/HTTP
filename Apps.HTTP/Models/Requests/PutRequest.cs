using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public class PutRequest : Request
{
    [Display("Request body in JSON format")]
    public string Body { get; set; }
    
    [Display("Query parameters in JSON format")]
    public string? QueryParameters { get; set; }
}