using OctoMessager.Client.Models.Content;

namespace OctoMessager.Client.Models.Webhooks
{
    public class V1MessageSent : V1WebhookEvent, IMessage, IMessageReceived
    {
        public override string EventType => WebhookEventType.MessageReceived;
        public required V1MessageBase Message { get; set; }
        public string MessageID => Message.MessageInfo.MessageID;
        public string Text => Message.Text;
        public string SenderID => Message.MessageInfo.User?.Id ?? throw new Exception("Invalid data received from webhook: messageReceived must always have sending user");
    }
}