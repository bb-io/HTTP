using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using RestSharp;

namespace Apps.HTTP.Models.Responses;

public class FileResponseDto : ResponseDto
{
    private FileResponseDto(RestResponse response) : base(response) { }

    [Display("Content file")]
    public FileReference ContentFile { get; set; }

    public static async Task<FileResponseDto> FromResponseAsync(RestResponse response, IFileManagementClient fileManagementClient)
    {
        var dto = new FileResponseDto(response);

        var contentTypeHeader = response.ContentHeaders?
            .FirstOrDefault(x => x.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();

        var contentType = GetContentType(contentTypeHeader);

        var contentDispositionHeader = response.ContentHeaders?
            .FirstOrDefault(h => h.Name.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();

        var fileName = GetFileName(contentDispositionHeader);

        if (string.IsNullOrWhiteSpace(contentDispositionHeader) || !contentDispositionHeader.Contains("attachment", StringComparison.OrdinalIgnoreCase))
        {
            var fromUrl = GetFileNameFromUri(response.ResponseUri);
            if (!string.IsNullOrWhiteSpace(fromUrl))
                fileName = fromUrl;
        }

        if (IsOctetStream(contentType))
        {
            var inferred = InferContentTypeFromFileName(fileName);
            if (!string.IsNullOrWhiteSpace(inferred))
                contentType = inferred;
        }

        fileName = EnsureFileExtension(fileName, contentType);

        using var stream = new MemoryStream(response.RawBytes ?? Array.Empty<byte>());
        dto.ContentFile = await fileManagementClient.UploadAsync(stream, contentType, fileName);

        return dto;
    }

    private static string GetContentType(string? contentTypeHeader)
    {
        if (!string.IsNullOrWhiteSpace(contentTypeHeader))
        {
            var parts = contentTypeHeader.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
                return parts[0].Trim();
        }

        return MediaTypeNames.Application.Octet;
    }

    private static bool IsOctetStream(string contentType) =>
        contentType.Equals(MediaTypeNames.Application.Octet, StringComparison.OrdinalIgnoreCase);

    private static string GetFileName(string? contentDispositionHeader)
    {
        if (!string.IsNullOrEmpty(contentDispositionHeader) &&
            contentDispositionHeader.Contains("attachment", StringComparison.OrdinalIgnoreCase))
        {
            var quoteStart = contentDispositionHeader.IndexOf('"');
            var quoteEnd = quoteStart >= 0 ? contentDispositionHeader.IndexOf('"', quoteStart + 1) : -1;

            return (quoteStart >= 0 && quoteEnd > quoteStart)
                ? contentDispositionHeader.Substring(quoteStart + 1, quoteEnd - quoteStart - 1)
                : Guid.NewGuid().ToString();
        }

        return Guid.NewGuid().ToString();
    }

    private static string? GetFileNameFromUri(Uri? uri)
    {
        if (uri == null) return null;
        var last = uri.Segments.LastOrDefault()?.Trim('/');
        if (string.IsNullOrWhiteSpace(last)) return null;
        return Uri.UnescapeDataString(last);
    }

    private static string? InferContentTypeFromFileName(string fileName)
    {
        if (!Path.HasExtension(fileName)) return null;

        var provider = new FileExtensionContentTypeProvider();
        return provider.TryGetContentType(fileName, out var ct) ? ct : null;
    }

    private static string EnsureFileExtension(string fileName, string contentType)
    {
        if (Path.HasExtension(fileName))
            return fileName;

        if (IsOctetStream(contentType))
            return fileName;

        var parts = contentType.Split('/');
        if (parts.Length == 2)
        {
            var ext = parts[1].Trim();
            if (!string.IsNullOrWhiteSpace(ext))
                fileName += "." + ext;
        }

        return fileName;
    }
}