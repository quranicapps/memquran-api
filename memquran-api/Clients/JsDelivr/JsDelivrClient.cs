namespace QuranApi.Contracts;

public class JsDelivrClient : BaseHttpClient, ICdnClient
{
    private readonly ClientsSettings _clientsSettings;
    private readonly ILogger<JsDelivrClient> _logger;

    public JsDelivrClient(HttpClient httpClient, ClientsSettings clientsSettings, ILogger<JsDelivrClient> logger) : base(httpClient, logger)
    {
        _clientsSettings = clientsSettings;
        _logger = logger;
    }

    protected override string ServiceName => "JsDelivrService";
    
    public async Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!_clientsSettings.JsDelivrService.Enabled)
        {            
            _logger.LogWarning("SKIPPED HTTP call. Client setting is disabled in config");
            return "";
        }
        
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri($"gh/quranstatic/static@{_clientsSettings.JsDelivrService.Version}/{filePath}", UriKind.Relative),
            Method = HttpMethod.Get
        };
        
        return await SendAsync(httpRequest, cancellationToken);
    }
}