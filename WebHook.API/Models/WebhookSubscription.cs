namespace WebHook.API.Models
{
    public sealed record WebhookSubscription(Guid Id, string EventType, string WebhookUrl, DateTime CreatedAt);

    public sealed record CreateWebhookRequest(string EventType, string WebhookUrl); 
}
