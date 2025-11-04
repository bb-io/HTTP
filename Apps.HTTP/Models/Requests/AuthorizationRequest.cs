using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP.Models.Requests;

public class AuthorizationRequest
{
    [Display("Authorization header value")]
    public string AuthorizationHeaderValue { get; set; } = string.Empty;
}