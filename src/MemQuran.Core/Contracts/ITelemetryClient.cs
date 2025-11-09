namespace MemQuran.Core.Contracts;

public interface ITelemetryClient
{
    Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostLogAsync(string message, CancellationToken cancellationToken = default);
}