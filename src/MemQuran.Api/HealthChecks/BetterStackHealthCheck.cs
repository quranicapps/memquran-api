using MemQuran.Core.Contracts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MemQuran.Api.HealthChecks;

public class BetterStackHealthCheck(ITelemetryClient telemetryClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var healthResponse = await telemetryClient.GetHealthAsync(cancellationToken);
        var postLogResponse = await telemetryClient.PostLogAsync("Health Check from MemQuran API", cancellationToken);

        var data = new Dictionary<string, object>
        {
            { "StatusCode", healthResponse.StatusCode },
            { "HealthStatusCode", healthResponse.StatusCode },
            { "PostLogStatusCode", postLogResponse.StatusCode },
        };

        return healthResponse.IsSuccessStatusCode && postLogResponse.IsSuccessStatusCode
            ? HealthCheckResult.Healthy(data: data)
            : HealthCheckResult.Unhealthy($"Health Status Code: ({(int)healthResponse.StatusCode}); Post Log Status Code: ({(int)postLogResponse.StatusCode})", data: data);
    }
}