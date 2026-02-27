using System.Net.Mime;

namespace Apps.HTTP.Utils;

public static class HttpFileHelpers
{
    public static string NormalizeContentType(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return MediaTypeNames.Application.Octet;

        var parts = header.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].Trim() : MediaTypeNames.Application.Octet;
    }

    public static bool IsOctetStream(string contentType) =>
        string.Equals(contentType, MediaTypeNames.Application.Octet, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(contentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase);

    public static string? GetHeaderValue(HttpResponseMessage resp, string name)
    {
        if (resp.Headers.TryGetValues(name, out var v1))
            return v1.FirstOrDefault();

        if (resp.Content.Headers.TryGetValues(name, out var v2))
            return v2.FirstOrDefault();

        return null;
    }

    public static string? TryGetFileNameFromContentDisposition(string? cd)
    {
        if (string.IsNullOrWhiteSpace(cd)) return null;

        var fileNameStar = TryGetDispositionParam(cd, "filename*");
        if (!string.IsNullOrWhiteSpace(fileNameStar))
        {
            var parts = fileNameStar.Split("''", 2);
            var encoded = parts.Length == 2 ? parts[1] : fileNameStar;
            try
            {
                return Uri.UnescapeDataString(encoded.Trim().Trim('"'));
            }
            catch
            {
                return encoded.Trim().Trim('"');
            }
        }

        var fileName = TryGetDispositionParam(cd, "filename");
        if (!string.IsNullOrWhiteSpace(fileName))
            return fileName.Trim().Trim('"');

        return null;
    }

    private static string? TryGetDispositionParam(string cd, string key)
    {
        var parts = cd.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var p = part.Trim();
            if (!p.StartsWith(key, StringComparison.OrdinalIgnoreCase)) continue;

            var eq = p.IndexOf('=');
            if (eq < 0) return null;
            return p[(eq + 1)..].Trim();
        }
        return null;
    }

    public static string? TryGetFileNameFromUrl(Uri? uri)
    {
        if (uri == null) return null;

        var last = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (string.IsNullOrWhiteSpace(last)) return null;

        if (!last.Contains('.')) return last;
        return last;
    }

    public static string? TryGetExtFromUrl(Uri? uri)
    {
        if (uri == null) return null;

        var path = uri.AbsolutePath;
        var dot = path.LastIndexOf('.');
        if (dot < 0 || dot == path.Length - 1) return null;

        var ext = path[(dot + 1)..].Trim().ToLowerInvariant();
        if (ext.Length is < 1 or > 10) return null;
        if (ext.Any(ch => !char.IsLetterOrDigit(ch))) return null;

        return ext;
    }

    public static string EnsureExtension(string fileName, string ext)
    {
        if (Path.HasExtension(fileName)) return fileName;
        return $"{fileName}.{ext.TrimStart('.')}";
    }

    public static string MakeFallbackName() => Guid.NewGuid().ToString("N");
}