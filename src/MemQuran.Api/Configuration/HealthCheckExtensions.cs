using HealthChecks.UI.Core;
using MemQuran.Api.HealthChecks;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using MemQuran.Core.Models;
using static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class HealthCheckExtensions
{
    public class HealthCheckConfiguration
    {
        public ContentDeliverySettings ContentDeliverySettings { get; set; } = null!;
        public int HealthCheckTimeoutSeconds { get; set; }
        public string RedisConnectionString { get; set; } = null!;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, Action<HealthCheckConfiguration> configuration)
    {
        var config = new HealthCheckConfiguration();
        configuration(config);

        // Health Checks and Health Checks UI (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
        var healthChecksBuilder = services.AddHealthChecks();
        healthChecksBuilder.AddCheck("API Running", () => Healthy(), tags: [nameof(HealthCheckTags.Local)]);

        if (config.ContentDeliverySettings.ContentDeliveryType == ContentDeliveryType.JsDelivr)
        {
            healthChecksBuilder.AddCheck<JsDelivrHealthCheck>("Call JsDelivr", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Cdn)]);
        }

        if (config.ContentDeliverySettings.ContentDeliveryType == ContentDeliveryType.Local)
        {
            healthChecksBuilder.AddCheck<LocalCdnHealthCheck>("Call Local CDN", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Cdn)]);
        }

        healthChecksBuilder.AddUrlGroup(new Uri("https://mock.httpstatus.io/200"), name: "https://mock.httpstatus.io/200", tags: [nameof(HealthCheckTags.Ping)]);

        if (config.ContentDeliverySettings.CachingSettings.CacheType == CacheType.Hybrid)
        {
            healthChecksBuilder.AddRedis(config.RedisConnectionString, "Call Redis", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Redis)]);
        }

        services.AddHealthChecksUI(setup =>
        {
            setup.SetHeaderText("Health Checks Status - MemQuran.API");
            setup.SetEvaluationTimeInSeconds(30);
            setup.SetMinimumSecondsBetweenFailureNotifications(60);
            setup.MaximumHistoryEntriesPerEndpoint(100);
            setup.SetApiMaxActiveRequests(1);

            var tags = Enum.GetValues<HealthCheckTags>().Select(x => x.ToString()).ToList();
            tags.ForEach(x => setup.AddHealthCheckEndpoint(x, $"http://localhost:3122/api/health/{x}"));

            setup.AddWebhookNotification("Webhook (https://memquran-api.requestcatcher.com)", uri: "https://memquran.requestcatcher.com/anything",
                payload: "{ \"message\": \"Webhook report for [[LIVENESS]] Health Check: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"}",
                restorePayload: "{ \"message\": \"[[LIVENESS]] Health Check is back to life\"}",
                shouldNotifyFunc: (livenessName, report) => DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour <= 23,
                customMessageFunc: (livenessName, report) =>
                {
                    var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                    return $"{failing.Count()} healthchecks are failing";
                }, customDescriptionFunc: (livenessName, report) =>
                {
                    var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                    return $"{string.Join(" - ", failing.Select(f => f.Key))} healthchecks are failing";
                });
        }).AddInMemoryStorage();

        return services;
    }
}