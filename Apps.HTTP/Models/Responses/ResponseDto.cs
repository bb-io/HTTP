using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using RestSharp;
using File = Blackbird.Applications.Sdk.Common.Files.File;

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
            .ToDictionary(x => x.Name!, x => x.Value?.ToString());
    }

    public Dictionary<string,string?>? Headers { get; set; }

    [Display("Status code")]
    public string StatusCode { get; set; }
    
    public string? Content { get; set; }
    
    [Display("Content type")]
    public string? ContentType { get; set; }
}