using WebHook.API.Repositories;

namespace WebHook.API.Services
{
    public class WebhookDispatcher(HttpClient httpClient, InMemoryWebhookSubscriptionRepository subscriptionRepository)
    {
        public async Task DispatcherAsync(string eventType, object payload)
        {
            var subscriptions = subscriptionRepository.GetByEventType(eventType);
            foreach (var subscription in subscriptions)
            {
                //var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                //await httpClient.PostAsync(subscription.WebhookUrl, content);
                var request = new
                {
                    Id = Guid.NewGuid(),
                    subscription.EventType,
                    SubscriptionId = subscription.Id,
                    Timestamp = DateTime.UtcNow,
                    Data = payload
                };
                await httpClient.PostAsJsonAsync(subscription.WebhookUrl, request);
            }

        }
    }
}
