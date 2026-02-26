using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
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

    private FileResponseDto(RestResponse response) : base(response)
    {
    }

    [Display("Content file")]
    public FileReference ContentFile { get; set; }

    public static async Task<FileResponseDto> FromStreamAsync(
        RestResponse meta,
        Stream contentStream,
        IFileManagementClient fileManagementClient)
    {
        var dto = new FileResponseDto(meta);

        var contentTypeHeader = meta.ContentHeaders?
            .FirstOrDefault(x => x.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();

        var contentType = NormalizeContentType(contentTypeHeader);

        var contentDispositionHeader = meta.ContentHeaders?
            .FirstOrDefault(h => h.Name.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();

        var fileName = GetFileNameFromContentDisposition(contentDispositionHeader);

        if (string.IsNullOrWhiteSpace(fileName))
            fileName = GetFileNameFromUri(meta.ResponseUri);

        if (string.IsNullOrWhiteSpace(fileName))
            fileName = Guid.NewGuid().ToString();

        if (IsOctetStream(contentType))
        {
            var inferred = InferContentTypeFromFileName(fileName);
            if (!string.IsNullOrWhiteSpace(inferred))
                contentType = inferred;
        }

        fileName = EnsureFileExtension(fileName, contentType);

        dto.ContentFile = await fileManagementClient.UploadAsync(contentStream, contentType, fileName);
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
            if (!fileName.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase) && ext.ToLower() != "octet-stream")
                fileName += "." + ext;
        }

        return fileName;
    }

    private static string NormalizeContentType(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return MediaTypeNames.Application.Octet;

        var parts = header.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].Trim() : MediaTypeNames.Application.Octet;
    }

    private static bool IsOctetStream(string contentType) =>
        contentType.Equals(MediaTypeNames.Application.Octet, StringComparison.OrdinalIgnoreCase);

    private static string? GetFileNameFromUri(Uri? uri)
    {
        if (uri == null) return null;
        var last = uri.Segments.LastOrDefault()?.Trim('/');
        if (string.IsNullOrWhiteSpace(last)) return null;
        return Uri.UnescapeDataString(last);
    }

    private static string? GetFileNameFromContentDisposition(string? cd)
    {
        if (string.IsNullOrWhiteSpace(cd)) return null;

        if (!cd.Contains("attachment", StringComparison.OrdinalIgnoreCase)) return null;

        var quoteStart = cd.IndexOf('"');
        var quoteEnd = quoteStart >= 0 ? cd.IndexOf('"', quoteStart + 1) : -1;

        if (quoteStart >= 0 && quoteEnd > quoteStart)
            return cd.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);

        return null;
    }

    private static string? InferContentTypeFromFileName(string fileName)
    {
        if (!Path.HasExtension(fileName)) return null;

        var provider = new FileExtensionContentTypeProvider();
        return provider.TryGetContentType(fileName, out var ct) ? ct : null;
    }
}