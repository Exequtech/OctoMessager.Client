using OctoMessager.Client.Models.Webhooks;
using System.Text.Json;

namespace OctoMessager.Client.Configuration
{
    public class OctoClientConfig
    {
        public string BaseUrl { get; set; } = "https://octo-messager.exequtech.com";
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            Converters = { new WebhookEventConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
