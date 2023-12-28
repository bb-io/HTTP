using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HTTP.Models.Responses;

public class FileResponseDto : ResponseDto
{
    public FileResponseDto(RestResponse response, IFileManagementClient fileManagementClient) : base(response)
    {
        var contentDisposition = response.ContentHeaders.FirstOrDefault(header => header.Name == "Content-Disposition");
        var filename = contentDisposition != null && contentDisposition.Value.ToString().Contains("attachment;")
            ? contentDisposition.Value.ToString().Split('"')[1]
            : Guid.NewGuid().ToString();
        var contentType = response.ContentHeaders?.FirstOrDefault(x => x.Name == "Content-Type")?.Value?.ToString() ??
                          MediaTypeNames.Application.Octet;

        using var stream = new MemoryStream(response.RawBytes);
        ContentFile = fileManagementClient.UploadAsync(stream, contentType, filename).Result;
    }
    
    [Display("Content file")]
    public FileReference ContentFile { get; set; }
}