using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public abstract class Request
{
    public string Endpoint { get; set; }
    
    [Display("Headers in JSON format")]
    public string? Headers { get; set; }
}