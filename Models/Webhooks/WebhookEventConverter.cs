using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctoMessager.Client.Models.Webhooks
{
    public class WebhookEventConverter : JsonConverter<WebhookEventBase>
    {
        public override WebhookEventBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            var eventType = jsonDocument.RootElement.GetProperty("eventType").GetString();

            var jsonString = jsonDocument.RootElement.GetRawText();
            return eventType switch
            {
                WebhookEventType.MessageReceived => JsonSerializer.Deserialize<MessageReceivedEvent>(jsonString, options),
                WebhookEventType.MessageSent => JsonSerializer.Deserialize<MessageSentEvent>(jsonString, options),
                WebhookEventType.PollAnswer => JsonSerializer.Deserialize<PollAnswerEvent>(jsonString, options),
                _ => throw new JsonException($"Unknown event type: {eventType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, WebhookEventBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}