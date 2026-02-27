using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HTTP.Models.Responses;

public class FileResponseDto
{
    [Display("Content file")]
    public FileReference ContentFile { get; set; }

    public static async Task<FileResponseDto> Create(RestResponse response, Stream stream, IFileManagementClient fileManagementClient)
    {
        string contentTypeHeader = response.ContentHeaders?.FirstOrDefault(x => x.Name == "Content-Type")?.Value?.ToString() ?? string.Empty;
        string contentType = GetContentType(contentTypeHeader);

        string contentDisposition = response.ContentHeaders?.FirstOrDefault(h => h.Name == "Content-Disposition")?.Value?.ToString() ?? string.Empty;
        string fileName = GetFileName(contentDisposition);

        fileName = EnsureFileExtension(fileName, contentType, response.ResponseUri?.ToString());

        var fileReference = await fileManagementClient.UploadAsync(stream, contentType, fileName);

        return new FileResponseDto { ContentFile = fileReference };
    }

    private static string GetContentType(string? contentTypeHeader)
    {
        if (string.IsNullOrWhiteSpace(contentTypeHeader)) return MediaTypeNames.Application.Octet;
        return contentTypeHeader.Split(';')[0].Trim();
    }

    private static string GetFileName(string? contentDisposition)
    {
        if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
        {
            var parts = contentDisposition.Split("filename=");
            if (parts.Length > 1)
                return parts[1].Trim('"').Trim('\'');
        }
        return Guid.NewGuid().ToString();
    }

    private static string EnsureFileExtension(string fileName, string contentType, string? url)
    {
        if (contentType == MediaTypeNames.Application.Octet && !string.IsNullOrEmpty(url))
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            var lastSegment = path.Split('/').LastOrDefault();
            if (lastSegment != null && lastSegment.Contains('.'))
            {
                var ext = Path.GetExtension(lastSegment);
                if (!fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    return fileName + ext;
                return fileName;
            }
        }

        var parts = contentType.Split('/');
        if (parts.Length == 2)
        {
            var ext = parts[1].Trim();
            if (ext != "octet-stream" && !fileName.Contains("."))
                return $"{fileName}.{ext}";
        }

        return fileName;
    }
}