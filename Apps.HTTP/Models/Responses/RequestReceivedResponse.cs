using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Responses;

public class RequestReceivedResponse
{
    [Display("Body or query parameters")]
    public string Body { get; set; } = string.Empty;
}