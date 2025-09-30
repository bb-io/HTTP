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
        string? contentTypeHeader = response.ContentHeaders?.FirstOrDefault(x => x.Name == "Content-Type")?.Value?.ToString();
        string contentType = GetContentType(contentTypeHeader);

        string? contentDispositionHeader = response.ContentHeaders?.FirstOrDefault(h => h.Name == "Content-Disposition")?.Value?.ToString();
        string fileName = GetFileName(contentDispositionHeader);
        fileName = EnsureFileExtension(fileName, contentType);

        using var stream = new MemoryStream(response.RawBytes);
        ContentFile = fileManagementClient.UploadAsync(stream, contentType, fileName).Result;
    }
    
    [Display("Content file")]
    public FileReference ContentFile { get; set; }

    private static string GetContentType(string? contentTypeHeader)
    {
        if (!string.IsNullOrEmpty(contentTypeHeader))
        {
            return contentTypeHeader.Split(';')[0].Trim();
        }
        else
        {
            return MediaTypeNames.Application.Octet;
        }
    }

    private static string GetFileName(string? contentDispositionHeader)
    {
        if (!string.IsNullOrEmpty(contentDispositionHeader) && contentDispositionHeader.Contains("attachment"))
        {
            var quoteStart = contentDispositionHeader.IndexOf('"');
            var quoteEnd = quoteStart >= 0 ? contentDispositionHeader.IndexOf('"', quoteStart + 1) : -1;

            return (quoteStart >= 0 && quoteEnd > quoteStart)
                ? contentDispositionHeader.Substring(quoteStart + 1, quoteEnd - quoteStart - 1)
                : Guid.NewGuid().ToString();
        }
        else
        {
            return Guid.NewGuid().ToString();
        }
    }

    private static string EnsureFileExtension(string fileName, string contentType)
    {
        var parts = contentType.Split('/');
        if (parts.Length == 2)
        {
            var ext = parts[1].Trim();
            if (!fileName.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase))
                fileName += "." + ext;
        }

        return fileName;
    }
}