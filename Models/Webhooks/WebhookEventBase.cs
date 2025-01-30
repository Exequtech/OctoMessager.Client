namespace OctoMessager.Client.Models.Webhooks
{
    public abstract class WebhookEventBase
    {
        public required string EventId { get; set; }
        public required string EventType { get; set; }
        public required DateTime Timestamp { get; set; }
    }
}