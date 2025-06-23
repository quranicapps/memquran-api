using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MemQuran.Api.HealthChecks;

public class JsDelivrHealthCheck(ICdnClientFactory cdnClientFactory) : IHealthCheck
{
    private readonly ICdnClient _cdnClient = cdnClientFactory.Create(ContentDeliveryType.JsDelivr);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var response = await _cdnClient.GetHealthAsync(cancellationToken);

        return response.IsSuccessStatusCode
            ? HealthCheckResult.Healthy(data: new Dictionary<string, object>
            {
                { "StatusCode", response.StatusCode }
            })
            : HealthCheckResult.Unhealthy($"Status Code: {response.StatusCode} ({(int)response.StatusCode})", data: new Dictionary<string, object>
            {
                { "StatusCode", response.StatusCode }
            });
    }
}