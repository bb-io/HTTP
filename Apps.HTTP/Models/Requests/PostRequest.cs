using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public class PostRequest : Request
{
    [Display("Request body in JSON format")]
    public string Body { get; set; }
}