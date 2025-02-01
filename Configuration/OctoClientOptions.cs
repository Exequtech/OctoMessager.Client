using System.Text.Json;
using Microsoft.Extensions.Options;

using OctoMessager.Client.Models.Webhooks;

namespace OctoMessager.Client.Configuration
{
    public class OctoClientOptions
    {
        public const string SectionName = "OctoMessager";

        public required string ApiKey { get; set; }
        public int RetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            Converters = { new WebhookEventConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    // Validator to ensure configuration is valid
    public class OctoClientOptionsValidator : IValidateOptions<OctoClientOptions>
    {
        public ValidateOptionsResult Validate(string? name, OctoClientOptions options)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                errors.Add("ApiKey is required");
            }

            if (options.RetryCount < 0)
            {
                errors.Add("RetryCount must be greater than or equal to 0");
            }

            if (options.TimeoutSeconds <= 0)
            {
                errors.Add("TimeoutSeconds must be greater than 0");
            }

            return errors.Count > 0
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;
        }
    }
}
