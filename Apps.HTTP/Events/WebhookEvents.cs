using Apps.HTTP.Models.Requests;
using Apps.HTTP.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.HTTP.Events;

[WebhookList]
public class WebhookEvents(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    [Webhook("On request received", Description = "Triggered when an HTTP request is received")]
    public Task<WebhookResponse<RequestReceivedResponse>> OnRequestReceived(WebhookRequest webhookRequest,
        [WebhookParameter] AuthorizationRequest authorizationRequest)
    {
        var authHeader = webhookRequest.Headers?
            .FirstOrDefault(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).Value;
        if (!string.IsNullOrEmpty(authorizationRequest.AuthorizationHeaderValue))
        {
            if (authHeader != authorizationRequest.AuthorizationHeaderValue)
            {
                var unauthorizedJson = "{ \"message\": \"unauthorized\" }";
                return Task.FromResult(new WebhookResponse<RequestReceivedResponse>
                {
                    HttpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent(unauthorizedJson)
                    },
                    Result = new RequestReceivedResponse
                    {
                        Body = "Unauthorized"
                    },
                    ReceivedWebhookRequestType = WebhookRequestType.Preflight
                });
            }
        }
        
        var httpResponseMessage = new HttpResponseMessage
        {
            Content = new StringContent("Request received successfully")
        };

        httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        
        var webhookBody = webhookRequest.Body.ToString() ?? string.Empty;
        if (webhookRequest.HttpMethod == HttpMethod.Get)
        {
            var queryParams = webhookRequest.QueryParameters
                .Select(kv => $"{kv.Key}={kv.Value}")
                .ToList();
            webhookBody += queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
        }
        
        var finalResponse = new WebhookResponse<RequestReceivedResponse>
        {
            HttpResponseMessage = httpResponseMessage,
            Result = new RequestReceivedResponse
            {
                Body = webhookBody
            },
            ReceivedWebhookRequestType = WebhookRequestType.Default
        };
        
        return Task.FromResult(finalResponse);
    }
}