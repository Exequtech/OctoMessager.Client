using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net;

using OctoMessager.Client.Configuration;
using OctoMessager.Client.Interfaces;

namespace OctoMessager.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOctoMessager(
            this IServiceCollection services,
            Action<OctoClientOptions> configureOptions)
        {
            // Add options configuration
            services.AddOptions<OctoClientOptions>()
                .Configure(configureOptions)
                .ValidateOnStart();

            services.AddSingleton<IValidateOptions<OctoClientOptions>, OctoClientOptionsValidator>();

            // Configure HttpClient with Polly policies
            services.AddHttpClient<IOctoClient, OctoClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<OctoClientOptions>>().Value;

                    client.BaseAddress = new Uri("https://octo-messager.exequtech.com");
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                    client.DefaultRequestHeaders.Add("X-API-Key", options.ApiKey);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
