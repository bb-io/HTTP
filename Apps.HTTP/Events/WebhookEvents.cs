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
        var unauthorizedResponse = ValidateAuthorization(webhookRequest, authorizationRequest);
        if (unauthorizedResponse != null)
        {
            return Task.FromResult(unauthorizedResponse);
        }

        var successResponse = new HttpResponseMessage
        {
            Content = new StringContent("Request received successfully")
        };
        successResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        
        var requestBody = BuildRequestBody(webhookRequest);
        
        var webhookResponse = new WebhookResponse<RequestReceivedResponse>
        {
            HttpResponseMessage = successResponse,
            Result = new RequestReceivedResponse
            {
                Body = requestBody
            },
            ReceivedWebhookRequestType = WebhookRequestType.Default
        };
        
        return Task.FromResult(webhookResponse);
    }

    private WebhookResponse<RequestReceivedResponse>? ValidateAuthorization(
        WebhookRequest webhookRequest, 
        AuthorizationRequest authorizationRequest)
    {
        if (string.IsNullOrEmpty(authorizationRequest.AuthorizationHeaderValue))
        {
            return null;
        }

        var receivedAuthHeader = webhookRequest.Headers?
            .FirstOrDefault(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).Value;

        if (receivedAuthHeader == authorizationRequest.AuthorizationHeaderValue)
        {
            return null;
        }

        var errorJson = "{ \"message\": \"unauthorized\" }";
        var unauthorizedHttpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(errorJson)
        };
        
        unauthorizedHttpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return new WebhookResponse<RequestReceivedResponse>
        {
            HttpResponseMessage = unauthorizedHttpResponse,
            Result = new RequestReceivedResponse(),
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
    }

    private string BuildRequestBody(WebhookRequest webhookRequest)
    {
        var requestBody = webhookRequest.Body.ToString() ?? string.Empty;
        
        if (webhookRequest.HttpMethod == HttpMethod.Get)
        {
            var queryParams = webhookRequest.QueryParameters
                .Select(kv => $"{kv.Key}={kv.Value}")
                .ToList();
                
            requestBody += queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
        }
        
        return requestBody;
    }
}