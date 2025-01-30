namespace OctoMessager.Client.Webhooks
{
    public class MessagePayload
    {
        public string MessageId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}