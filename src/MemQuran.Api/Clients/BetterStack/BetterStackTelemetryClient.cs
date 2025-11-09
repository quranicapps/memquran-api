using System.Text.Json;
using MemQuran.Core.Contracts;

namespace MemQuran.Api.Clients.BetterStack;

public class BetterStackTelemetryClient(HttpClient httpClient, ILogger<BetterStackTelemetryClient> logger) : BaseHttpClient(httpClient, logger), ITelemetryClient
{
    private readonly HttpClient _httpClient = httpClient;
    protected override string ServiceName => "BetterStackTelemetryService";

    public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync("", cancellationToken);
    }

    public async Task<HttpResponseMessage> PostLogAsync(string message, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri("", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new { message }), System.Text.Encoding.UTF8, "application/json")
        };
        
        return await _httpClient.SendAsync(httpRequest, cancellationToken);
    }
}