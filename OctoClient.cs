using OctoMessager.client.Configuration;
using OctoMessager.client.Exceptions;
using OctoMessager.client.Interfaces;
using OctoMessager.client.Models.Content;
using OctoMessager.Client.Models.Content;
using OctoMessager.Client.Models.Requests;
using OctoMessager.Client.Models.Responses;
using System.Text;
using System.Text.Json;

namespace OctoMessager.client
{
    public class OctoClient : IOctoClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly OctoClientConfig _config;
        private bool _disposed;

        public OctoClient(string apiKey, OctoClientConfig? config = null)
        {
            _config = config ?? new OctoClientConfig();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_config.BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public async Task<MessageResponse> SendTextMessageAsync(string to, string message)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = message
            };

            return await SendRequestAsync(request);
        }

        public async Task<MessageResponse> SendImageMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = caption,
                Image = new MediaContent
                {
                    Data = data,
                    FileName = fileName
                },
                Delay = delay,
                PreviewLinks = previewLinks
            };

            return await SendRequestAsync(request);
        }

        public async Task<MessageResponse> SendVideoMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = caption,
                Video = new MediaContent
                {
                    Data = data,
                    FileName = fileName
                },
                Delay = delay,
                PreviewLinks = previewLinks
            };

            return await SendRequestAsync(request);
        }

        public async Task<MessageResponse> SendDocumentMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = caption,
                Document = new MediaContent
                {
                    Data = data,
                    FileName = fileName
                },
                Delay = delay,
                PreviewLinks = previewLinks
            };

            return await SendRequestAsync(request);
        }

        public async Task<MessageResponse> SendPollMessageAsync(string to, string[] options, bool multiSelectable, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = caption,
                Poll = new PollContent
                {
                    Options = options,
                    MultiSelectable = multiSelectable
                },
                Delay = delay,
                PreviewLinks = previewLinks
            };

            return await SendRequestAsync(request);
        }

        public async Task<MessageResponse> SendReferencedMediaAsync(string to, string messageId, string caption = "", int delay = 0, bool previewLinks = false)
        {
            var request = new MessageRequest
            {
                Id = to,
                Text = caption,
                AttachmentRef = new AttachmentReference
                {
                    MessageId = messageId
                },
                Delay = delay,
                PreviewLinks = previewLinks
            };

            return await SendRequestAsync(request);
        }

        private async Task<MessageResponse> SendRequestAsync<T>(T payload)
        {
            ValidateNotDisposed();

            try
            {
                var json = JsonSerializer.Serialize(payload, _config.JsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("v1/message/sendText", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new OctoMessengerException(
                        $"API request failed with status code {response.StatusCode}: {responseContent}",
                        response.StatusCode);
                }

                var messageResponse = JsonSerializer.Deserialize<MessageResponse>(responseContent, _config.JsonSerializerOptions);

                return messageResponse ?? throw new OctoMessengerException("Failed to deserialize the response from the server");
            }
            catch (Exception ex) when (ex is not OctoMessengerException)
            {
                throw new OctoMessengerException("An error occurred while sending the message", ex);
            }
        
        }

        private void ValidateNotDisposed() =>
            ObjectDisposedException.ThrowIf(_disposed, nameof(OctoClient));

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
