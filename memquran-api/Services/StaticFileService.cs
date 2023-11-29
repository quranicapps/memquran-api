using QuranApi.Settings;

namespace QuranApi.Contracts;

public class StaticFileService : IStaticFileService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly ICdnClient _cdnClient;
    private readonly ILogger<StaticFileService> _logger;

    public StaticFileService(ICachingProviderFactory cachingProviderFactory, ICdnClient cdnClient, ContentDeliverySettings contentDeliverySettings, ILogger<StaticFileService> logger)
    {
        _cachingProvider = cachingProviderFactory.GetCachingProvider(contentDeliverySettings.CachingSettings.CacheType);
        _cdnClient = cdnClient;
        _logger = logger;
    }

    public async Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var cacheKey = Path.GetFileName(filePath);
        var text = await _cachingProvider.GetStringAsync(cacheKey, cancellationToken);

        if (text is not null) return text;
        
        text = await _cdnClient.GetFileContentAsync(filePath, cancellationToken);
            
        if(text is not null)
        {
            await _cachingProvider.SetStringAsync(cacheKey, text, cancellationToken);
        }
        
        return text;
    }
}