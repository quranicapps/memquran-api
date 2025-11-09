using HealthChecks.UI.Core;
using MemQuran.Api.HealthChecks;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public class HealthCheckOptions
{
    public HealthCheckSettings HealthCheckSettings { get; set; } = null!;
    public PingSettings PingSettings { get; set; } = null!;
    public RedisSettings RedisSettings { get; set; } = null!;
}

public static class HealthCheckExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, Action<HealthCheckOptions> optionsFactory)
    {
        var options = new HealthCheckOptions();
        optionsFactory(options);

        var tags = new List<string>();

        // Health Checks and Health Checks UI (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
        var healthChecksBuilder = services.AddHealthChecks();

        tags.Add(nameof(HealthCheckTag.Local));
        healthChecksBuilder.AddCheck("API Running", () => Healthy(), tags: [nameof(HealthCheckTag.Local)]);

        if (options.HealthCheckSettings.Ping.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.Ping));
            foreach (var endpoint in options.PingSettings.ExternalEndpoints)
            {
                healthChecksBuilder.AddUrlGroup(new Uri(endpoint.Url), name: endpoint.Name, timeout: options.PingSettings.Timeout, tags: [nameof(HealthCheckTag.Ping)]);   
            }
        }

        if (options.HealthCheckSettings.LocalFiles.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.LocalFiles));
            healthChecksBuilder.AddCheck<LocalCdnHealthCheck>("Check Local Files", timeout: options.HealthCheckSettings.LocalFiles.TimeOut, tags: [nameof(HealthCheckTag.LocalFiles)]);
        }

        if (options.HealthCheckSettings.JsDelivr.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.Cdn));
            healthChecksBuilder.AddCheck<JsDelivrHealthCheck>("Call JsDelivr", timeout: options.HealthCheckSettings.JsDelivr.TimeOut, tags: [nameof(HealthCheckTag.Cdn)]);
        }

        if (options.HealthCheckSettings.Redis.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.DistributedCache));
            healthChecksBuilder.AddRedis(options.RedisSettings.ConnectionString, "Call Redis", timeout: options.HealthCheckSettings.Redis.TimeOut, tags: [nameof(HealthCheckTag.DistributedCache)]);
        }
        
        if (options.HealthCheckSettings.BetterStack.Enabled)
        {
            tags.Add(nameof(HealthCheckTag.Telemetry));
            healthChecksBuilder.AddCheck<BetterStackHealthCheck>("Call Better Stack", timeout: options.HealthCheckSettings.BetterStack.TimeOut, tags: [nameof(HealthCheckTag.Telemetry)]);
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