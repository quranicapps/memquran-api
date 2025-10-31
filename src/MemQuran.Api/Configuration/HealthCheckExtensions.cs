using HealthChecks.UI.Core;
using MemQuran.Api.HealthChecks;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public class WebConfiguration
{
    public HealthCheckSettings HealthCheckSettings { get; set; } = null!;
    public string RedisConnectionString { get; set; } = null!;
    public IWebHostEnvironment Environment { get; set; } = null!;
}

public static class HealthCheckExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, Action<WebConfiguration> configuration)
    {
        var config = new WebConfiguration();
        configuration(config);

        var tags = new List<string>();

        // Health Checks and Health Checks UI (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
        var healthChecksBuilder = services.AddHealthChecks();

        tags.Add(nameof(HealthCheckTag.Local));
        healthChecksBuilder.AddCheck("API Running", () => Healthy(), tags: [nameof(HealthCheckTag.Local)]);

        tags.Add(nameof(HealthCheckTag.Ping));
        healthChecksBuilder.AddUrlGroup(new Uri("https://mock.httpstatus.io/200"), name: "https://mock.httpstatus.io/200", tags: [nameof(HealthCheckTag.Ping)]);

        if (config.HealthCheckSettings.LocalFiles.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.LocalFiles));
            healthChecksBuilder.AddCheck<LocalCdnHealthCheck>("Check Local Files", timeout: config.HealthCheckSettings.LocalFiles.TimeOut, tags: [nameof(HealthCheckTag.LocalFiles)]);
        }

        if (config.HealthCheckSettings.JsDelivr.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.JsDelivr));
            healthChecksBuilder.AddCheck<LocalCdnHealthCheck>("Call JsDelivr", timeout: config.HealthCheckSettings.JsDelivr.TimeOut, tags: [nameof(HealthCheckTag.JsDelivr)]);
        }

        if (config.HealthCheckSettings.Redis.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.Redis));
            healthChecksBuilder.AddRedis(config.RedisConnectionString, "Call Redis", timeout: config.HealthCheckSettings.Redis.TimeOut, tags: [nameof(HealthCheckTag.Redis)]);
        }

        services.AddHealthChecksUI(setup =>
        {
            setup.SetHeaderText("Health Checks Status - MemQuran.API");
            setup.SetEvaluationTimeInSeconds(30);
            setup.SetMinimumSecondsBetweenFailureNotifications(60);
            setup.MaximumHistoryEntriesPerEndpoint(100);
            setup.SetApiMaxActiveRequests(1);

            tags.Distinct().ToList().ForEach(x => setup.AddHealthCheckEndpoint(x, $"http://localhost:3122/api/health/{x}"));

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