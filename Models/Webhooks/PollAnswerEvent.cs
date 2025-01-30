// PollAnswerEvent.cs
namespace OctoMessager.Client.Models.Webhooks
{
    public class PollAnswerEvent : WebhookEventBase
    {
        public required string MessageId { get; set; }
        public required string FromId { get; set; }
        public required string[] SelectedOptions { get; set; }
    }
}