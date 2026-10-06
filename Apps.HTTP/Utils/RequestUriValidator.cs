using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.HTTP.Utils;

internal static class RequestUriValidator
{
    private const string RequestUrlTooLongMessage =
        "Request URL is too long. Shorten the endpoint or query parameter values. " +
        "Large data should be sent in the request body instead of the URL.";

    private const string InvalidRequestUrlMessage =
        "Request URL is invalid. Check the Base URL, endpoint, and query parameter values.";

    public static Uri Create(Uri baseUri, string relativeUri)
    {
        try
        {
            return new Uri(baseUri, relativeUri);
        }
        catch (UriFormatException exception)
        {
            throw CreateMisconfigurationException(exception);
        }
    }

    public static PluginMisconfigurationException CreateMisconfigurationException(UriFormatException exception)
    {
        var message = exception.Message.Contains("too long", StringComparison.OrdinalIgnoreCase)
            ? RequestUrlTooLongMessage
            : InvalidRequestUrlMessage;

        return new PluginMisconfigurationException(message);
    }
}
