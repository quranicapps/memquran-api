using HealthChecks.Redis;
using HealthChecks.UI.Core;
using MemQuran.Api.Configuration.ApiServices;
using MemQuran.Api.HealthChecks;
using MemQuran.Api.Models;
using static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ApiHealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, Action<ApiConfiguration> configuration)
    {
        var config = new ApiConfiguration();
        configuration(config);
        
        // Health Checks and Health Checks UI (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
        services.AddHealthChecks()
            .AddCheck("API Running", () => Healthy(), tags: [nameof(HealthCheckTags.Local)])
            .AddCheck<JsDelivrHealthCheck>("Call JsDelivr", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Cdn)])
            .AddCheck<LocalCdnHealthCheck>("Call Local CDN", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Cdn)])
            .AddUrlGroup(new Uri("https://mock.httpstatus.io/200"), name: "https://mock.httpstatus.io/200", tags: [nameof(HealthCheckTags.Ping)])
            .AddRedis(config.RedisConnectionString, "Call Redis", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: [nameof(HealthCheckTags.Redis)]);
        
        services.AddHealthChecksUI(setup =>
        {
            setup.SetHeaderText("Health Checks Status - MemQuran.API");
            setup.SetEvaluationTimeInSeconds(30);
            setup.SetMinimumSecondsBetweenFailureNotifications(60);
            setup.MaximumHistoryEntriesPerEndpoint(100);
            setup.SetApiMaxActiveRequests(1);
            
            var tags = Enum.GetValues<HealthCheckTags>().Select(x => x.ToString()).ToList();
            tags.ForEach(x => setup.AddHealthCheckEndpoint(x, $"https://localhost:3123/api/health/{x}"));
            
            setup.AddWebhookNotification("Webhook (https://memquran.requestcatcher.com)", uri: "https://memquran.requestcatcher.com/anything",
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