using Blackbird.Applications.Sdk.Common;
using RestSharp;

namespace Apps.HTTP.Models;

public class ResponseDto
{
    public ResponseDto(RestResponse response)
    {
        StatusCode = response.StatusCode.ToString();
        Content = response.Content;
        ContentType = response.ContentType;
        RawBytes = response.RawBytes;
    }
    
    [Display("Status code")]
    public string StatusCode { get; set; }
    
    public string? Content { get; set; }
    
    [Display("Content type")]
    public string? ContentType { get; set; }
    
    [Display("Raw bytes")]
    public byte[]? RawBytes { get; set; }
}