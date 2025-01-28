using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OctoMessager.Client
{
    public class OctoClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://octo-messager.exequtech.com";

        public OctoClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public async Task<MessageResponse> SendTextMessageAsync(string to, string message)
        {
            var payload = new
            {
                id = to,
                text = message
            };

            return await SendRequestAsync(payload);
        }

        public async Task<MessageResponse> SendImageMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var payload = new
            {
                id = to,
                text = caption,
                image = new
                {
                    data,
                    fileName,
                },
                delay,
                previewLinks
            };

            return await SendRequestAsync(payload);
        }

        public async Task<MessageResponse> SendVideoMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var payload = new
            {
                id = to,
                text = caption,
                video = new
                {
                    data,
                    fileName,
                },
                delay,
                previewLinks
            };

            return await SendRequestAsync(payload);
        }

        public async Task<MessageResponse> SendDocumentMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var payload = new
            {
                id = to,
                text = caption,
                document = new
                {
                    data,
                    fileName,
                },
                delay,
                previewLinks
            };

            return await SendRequestAsync(payload);
        }

        public async Task<MessageResponse> SendPollMessageAsync(string to, string[] options, bool multiSelectable, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var payload = new
            {
                id = to,
                text = caption,
                poll = new
                {
                    options,
                    multiSelectable,
                },
                delay,
                previewLinks
            };

            return await SendRequestAsync(payload);
        }

        public async Task<MessageResponse> SendReferencedMediaAsync(string to, string messageId, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var payload = new
            {
                id = to,
                text = caption,
                attachmentRef = new
                {
                    messageID = messageId,
                },
                delay,
                previewLinks
            };

            return await SendRequestAsync(payload);
        }

        private async Task<MessageResponse> SendRequestAsync<T>(T payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/v1/message/sendText", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new OctoMessengerException($"API request failed with status code {response.StatusCode}: {responseContent}");
            }

            return JsonSerializer.Deserialize<MessageResponse>(responseContent);
        }
    }

    public class MessageResponse
    {
        [JsonPropertyName("messageID")]
        public string MessageId { get; set; }
    }

    public class OctoMessengerException : Exception
    {
        public OctoMessengerException(string message) : base(message) { }
    }

    public enum MediaType
    {
        Image,
        Video,
        Document
    }
}