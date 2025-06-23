using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;
using MemQuran.Core.Models;

namespace MemQuran.Api.Clients.JsDelivr;

public class JsDelivrClient(HttpClient httpClient, ClientsSettings clientsSettings, ILogger<JsDelivrClient> logger) : BaseHttpClient(httpClient, logger), ICdnClient
{
    protected override string ServiceName => "JsDelivrService";

    public ContentDeliveryType Name => ContentDeliveryType.JsDelivr;

    public async Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!clientsSettings.JsDelivrService.Enabled)
        {            
            logger.LogWarning("SKIPPED HTTP call. Client setting is disabled in config");
            return "";
        }
        
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"gh/quranstatic/static@{clientsSettings.JsDelivrService.Version}/{filePath}", UriKind.Relative),
            Method = HttpMethod.Get
        };
        
        return await SendAsync(httpRequest, cancellationToken);
    }

    public async Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!clientsSettings.JsDelivrService.Enabled)
        {            
            logger.LogWarning("SKIPPED HTTP call. Client setting is disabled in config");
            return null;
        }
        
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"gh/quranstatic/static@{clientsSettings.JsDelivrService.Version}/{filePath}", UriKind.Relative),
            Method = HttpMethod.Get
        };
        
        return await GetBytesAsync(httpRequest, cancellationToken);
    }

    public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        // throw new HttpServiceException("JsDelivrClient does not support readiness check", new HttpRequestMessage(), HttpStatusCode.NotImplemented, "WHAT");
        return await GetAsync($"gh/quranstatic/static@{clientsSettings.JsDelivrService.Version}/health.json", cancellationToken);
    }
}