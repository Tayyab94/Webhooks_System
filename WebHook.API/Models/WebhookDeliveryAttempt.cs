namespace WebHook.API.Models
{
    public class WebhookDeliveryAttempt
    {

        public Guid Id { get; set; }
        public Guid WebhookSubscriptionId { get; set; }
        public string Payload { get; set; }
        public int? ResposeStatusCode { get; set; }
        public bool Success { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
