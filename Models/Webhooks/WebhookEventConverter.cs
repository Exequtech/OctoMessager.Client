using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctoMessager.Client.Models.Webhooks
{
    public class WebhookEventConverter : JsonConverter<IWebhookEvent>
    {
        public override IWebhookEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            string eventType = jsonDocument.RootElement.GetProperty("event").GetString() ?? throw new JsonException("\"event\" is required for all events.");
            string[] data = eventType.Split("/");
            if (data[0] != "v1")
            {
                throw new JsonException($"Unsupported event version: '{data[0]}'");
            }
            string dataObj = jsonDocument.RootElement.GetProperty("data").GetRawText();
            switch(data[1])
            {
                case WebhookEventType.MessageReceived:
                    {
                        V1MessageBase msg = JsonSerializer.Deserialize<V1MessageBase>(dataObj) ?? throw new JsonException("Invalid msg received body");
                        return new V1MessageReceived() { Message = msg };
                    }
                case WebhookEventType.MessageSent:
                    {
                        V1MessageBase msg = JsonSerializer.Deserialize<V1MessageBase>(dataObj) ?? throw new JsonException("Invalid msg sent body");
                        return new V1MessageSent() { Message = msg };
                    }   
                default:
                    throw new JsonException("Invalid event type");
            }
        }

        public override void Write(Utf8JsonWriter writer, IWebhookEvent value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}