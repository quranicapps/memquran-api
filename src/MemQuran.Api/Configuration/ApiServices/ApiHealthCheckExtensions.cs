using MemQuran.Api.Configuration.ApiServices;
using MemQuran.Api.HealthChecks;
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
            .AddCheck("API Running", () => Healthy(), tags: ["health"])
            .AddCheck<JsDelivrHealthCheck>("Call JsDelivr", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: new List<string> { "services", "cdn" })
            .AddCheck<LocalCdnHealthCheck>("Call Local CDN", timeout: TimeSpan.FromSeconds(config.HealthCheckTimeoutSeconds), tags: new List<string> { "services", "cdn" })
            .AddUrlGroup(new Uri("http://httpbin.org/status/200"), name: "http connection check", tags: new List<string> { "services", "http", "internet" })
            .AddUrlGroup(new Uri("https://httpbin.org/status/200"), name: "https connection check", tags: new List<string> { "services", "https", "internet" });
        
        services.AddHealthChecksUI().AddInMemoryStorage();

        return services;
    }
}