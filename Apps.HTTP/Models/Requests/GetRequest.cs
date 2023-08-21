using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public class GetRequest : Request
{
    [Display("Query parameters in JSON format")]
    public string? QueryParameters { get; set; }
}