namespace OctoMessager.Client.Models.Webhooks
{
    public static class WebhookEventType
    {
        public const string MessageReceived = "message.received";
        public const string MessageSent = "message.sent";
        public const string PollAnswer = "poll.answer";
    }
}