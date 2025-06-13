using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;

namespace MemQuran.Api.Clients.JsDelivr;

public class JsDelivrClient(HttpClient httpClient, ClientsSettings clientsSettings, ILogger<JsDelivrClient> logger) : BaseHttpClient(httpClient, logger), ICdnClient
{
    protected override string ServiceName => "JsDelivrService";
    
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
}