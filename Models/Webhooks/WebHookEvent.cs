
namespace OctoMessager.Client.Webhooks
{
    public class WebhookEvent
    {
        public WebhookEventType EventType { get; set; }
        public string SourceId { get; set; }
        public DateTime Timestamp { get; set; }
        public dynamic Payload { get; set; }  // Use specific type if known
    }
}