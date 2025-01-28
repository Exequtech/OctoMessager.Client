using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OctoMessager.Client
{
    public interface IOctoClient
    {
        Task<MessageResponse> SendTextMessageAsync(string to, string message, CancellationToken cancellationToken = default);
        Task<MessageResponse> SendMediaMessageAsync(string to, string mediaUrl, MediaType mediaType, string caption = null, CancellationToken cancellationToken = default);
        Task<MessageResponse> SendTemplateMessageAsync(string to, string templateName, object parameters = null, CancellationToken cancellationToken = default);
        Task<MessageStatus> GetMessageStatusAsync(string messageId, CancellationToken cancellationToken = default);
    }

    public class OctoClientOptions
    {
        public string BaseUrl { get; set; } = "https://octo-messager.exequtech.com";
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxRetries { get; set; } = 3;
        public bool ValidatePhoneNumbers { get; set; } = true;
    }

    public class OctoClient : IOctoClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly OctoClientOptions _options;
        private bool _disposed;

        public OctoClient(string apiKey, OctoClientOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            _options = options ?? new OctoClientOptions();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_options.BaseUrl),
                Timeout = _options.Timeout
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "OctoMessenger-NET/1.0");
        }

        public async Task<MessageResponse> SendTextMessageAsync(
            string to,
            string message,
            CancellationToken cancellationToken = default)
        {
            ValidatePhoneNumber(to);
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            var payload = new MessageRequest
            {
                PhoneNumber = to,
                Type = "text",
                Content = message
            };

            return await SendRequestAsync<MessageResponse>("/messages", payload, HttpMethod.Post, cancellationToken);
        }

        public async Task<MessageResponse> SendMediaMessageAsync(
            string to,
            string mediaUrl,
            MediaType mediaType,
            string caption = null,
            CancellationToken cancellationToken = default)
        {
            ValidatePhoneNumber(to);
            if (string.IsNullOrWhiteSpace(mediaUrl))
                throw new ArgumentException("Media URL cannot be null or empty", nameof(mediaUrl));
            if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid media URL format", nameof(mediaUrl));

            var payload = new MediaMessageRequest
            {
                PhoneNumber = to,
                Type = mediaType.ToString().ToLower(),
                MediaUrl = mediaUrl,
                Caption = caption
            };

            return await SendRequestAsync<MessageResponse>("/messages", payload, HttpMethod.Post, cancellationToken);
        }

        public async Task<MessageResponse> SendTemplateMessageAsync(
            string to,
            string templateName,
            object parameters = null,
            CancellationToken cancellationToken = default)
        {
            ValidatePhoneNumber(to);
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Template name cannot be null or empty", nameof(templateName));

            var payload = new TemplateMessageRequest
            {
                PhoneNumber = to,
                Type = "template",
                TemplateName = templateName,
                Parameters = parameters
            };

            return await SendRequestAsync<MessageResponse>("/messages", payload, HttpMethod.Post, cancellationToken);
        }

        public async Task<MessageStatus> GetMessageStatusAsync(
            string messageId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(messageId))
                throw new ArgumentException("Message ID cannot be null or empty", nameof(messageId));

            return await SendRequestAsync<MessageStatus>($"/messages/{messageId}/status", null, HttpMethod.Get, cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(
            string endpoint,
            object payload,
            HttpMethod method = null,
            CancellationToken cancellationToken = default)
        {
            method ??= HttpMethod.Post;
            var retryCount = 0;

            while (true)
            {
                try
                {
                    using var request = new HttpRequestMessage(method, endpoint);
                    if (payload != null)
                    {
                        var json = JsonSerializer.Serialize(payload);
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    }

                    using var response = await _httpClient.SendAsync(request, cancellationToken);
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<T>(responseContent);
                    }

                    var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                    throw new OctoMessengerException(
                        error?.Message ?? $"API request failed with status code {response.StatusCode}",
                        error?.Code,
                        response.StatusCode);
                }
                catch (Exception ex) when (ShouldRetry(ex, ++retryCount))
                {
                    await Task.Delay(GetRetryDelay(retryCount), cancellationToken);
                }
            }
        }

        private bool ShouldRetry(Exception ex, int retryCount)
        {
            return retryCount < _options.MaxRetries && (
                ex is HttpRequestException ||
                ex is TimeoutException ||
                (ex is OctoMessengerException apiEx && (
                    (int)apiEx.StatusCode >= 500 ||
                    apiEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                ))
            );
        }

        private TimeSpan GetRetryDelay(int retryCount)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, retryCount - 1));
        }

        private void ValidatePhoneNumber(string phoneNumber)
        {
            if (!_options.ValidatePhoneNumbers)
                return;

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));

            if (!phoneNumber.StartsWith('+'))
                throw new ArgumentException("Phone number must start with '+' and include country code", nameof(phoneNumber));

            if (!phoneNumber.Skip(1).All(char.IsDigit))
                throw new ArgumentException("Phone number can only contain digits after the '+' symbol", nameof(phoneNumber));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }

    public class MessageRequest
    {
        public string PhoneNumber { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }

    public class MediaMessageRequest : MessageRequest
    {
        public string MediaUrl { get; set; }
        public string Caption { get; set; }
    }

    public class TemplateMessageRequest : MessageRequest
    {
        public string TemplateName { get; set; }
        public object Parameters { get; set; }
    }

    public class MessageResponse
    {
        public string MessageId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Cost { get; set; }
    }

    public class MessageStatus
    {
        public string MessageId { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Error { get; set; }
    }

    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class OctoMessengerException(string message, string errorCode = null, System.Net.HttpStatusCode statusCode = default)
        : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
        public System.Net.HttpStatusCode StatusCode { get; } = statusCode;
    }

    public enum MediaType
    {
        Image,
        Video,
        Document
    }
}