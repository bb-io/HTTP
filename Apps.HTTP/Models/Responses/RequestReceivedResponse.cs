using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Responses;

public class RequestReceivedResponse
{
    [Display("Body")]
    public string Body { get; set; } = string.Empty;
}