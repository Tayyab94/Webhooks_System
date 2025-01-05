using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebHook.API.Data;
using WebHook.API.Models;

namespace WebHook.API.Services
{
    internal sealed class WebhookDispatcher(IHttpClientFactory httpClientFactory, WebhookContext context)
    {
        public async Task DispatcherAsync<T>(string eventType, T data)
        {
            var subscriptions = await context.WebhookSubscriptions.AsNoTracking()
                .Where(s => s.EventType == eventType).ToListAsync();

            foreach (var subscription in subscriptions)
            {
                //var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                //await httpClient.PostAsync(subscription.WebhookUrl, content);

                using var httpClient = httpClientFactory.CreateClient();

                var payload = new WebhookPayload<T>
                {
                    Id = Guid.NewGuid(),
                   EventType= subscription.EventType,
                    SubscriptionId = subscription.Id,
                    TimeStemp = DateTime.UtcNow,
                    Data = data
                };

                var jsonPayload = JsonSerializer.Serialize(payload);

                try
                {
                    var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);

                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        WebhookSubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResposeStatusCode = (int)response.StatusCode,
                        Success = response.IsSuccessStatusCode,
                        TimeStamp = DateTime.UtcNow
                    };

                    context.WebhookDeliveryAttempts.Add(attempt);
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        WebhookSubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResposeStatusCode =null,
                        Success = false,
                        TimeStamp = DateTime.UtcNow
                    };

                    context.WebhookDeliveryAttempts.Add(attempt);
                    await context.SaveChangesAsync();
                }
              
            }

        }
    }

}
