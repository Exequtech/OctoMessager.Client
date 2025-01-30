namespace OctoMessager.Client.Models.Webhooks
{
    public class WebhookPayload
    {
        public required string WebhookId { get; set; }
        public required WebhookEventBase Event { get; set; }
    }
}