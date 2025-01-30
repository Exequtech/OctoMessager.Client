using OctoMessager.Client.Models.Content;

namespace OctoMessager.Client.Models.Webhooks
{
    public class MessageSentEvent : WebhookEventBase
    {
        public required string MessageId { get; set; }
        public required string ToId { get; set; }
        public required string Text { get; set; }
        public MediaContent? Image { get; set; }
        public MediaContent? Video { get; set; }
        public MediaContent? Document { get; set; }
        public PollContent? Poll { get; set; }
        public bool HasPreviewLinks { get; set; }
    }
}