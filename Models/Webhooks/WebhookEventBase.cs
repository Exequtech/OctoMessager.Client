namespace OctoMessager.Client.Models.Webhooks
{
    public interface IWebhookEvent
    {
        public string EventVersion { get; }
        public string EventType { get; }
    }
    public abstract class V1WebhookEvent : IWebhookEvent
    {
        public string EventVersion => "v1";
        public abstract string EventType { get; }
    }
}