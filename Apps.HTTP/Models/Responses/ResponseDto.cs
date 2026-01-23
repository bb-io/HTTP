using Blackbird.Applications.Sdk.Common;
using RestSharp;

namespace Apps.HTTP.Models.Responses;

public class ResponseDto
{
    public ResponseDto(RestResponse response)
    {
        StatusCode = response.StatusCode.ToString();
        Content = response.Content;
        ContentType = response.ContentType;
        Headers = response.Headers?
            .DistinctBy(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new HeaderDto(x.Name!, x.Value?.ToString()))
            .ToList() ?? new List<HeaderDto>();
    }

    public List<HeaderDto> Headers { get; set; }

    [Display("Status code")]
    public string StatusCode { get; set; }
    
    public string? Content { get; set; }
    
    [Display("Content type")]
    public string? ContentType { get; set; }
}