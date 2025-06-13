using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;

namespace MemQuran.Api.Services;

public class StaticFileService(
    ICachingProviderFactory cachingProviderFactory, 
    ICdnClient cdnClient, 
    ContentDeliverySettings contentDeliverySettings, 
    ILogger<StaticFileService> logger)
    : IStaticFileService
{
    private readonly ICachingProvider _cachingProvider = cachingProviderFactory.GetCachingProvider(contentDeliverySettings.CachingSettings.CacheType);
    private readonly ILogger<StaticFileService> _logger = logger;

    public async Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var cacheKey = Path.GetFileName(filePath);
        var text = await _cachingProvider.GetStringAsync(cacheKey, cancellationToken);

        if (text is not null) return text;
        
        text = await cdnClient.GetFileContentStringAsync(filePath, cancellationToken);
            
        if(text is not null)
        {
            await _cachingProvider.SetStringAsync(cacheKey, text, cancellationToken);
        }
        
        return text;
    }

    public async Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await cdnClient.GetFileContentBytesAsync(filePath, cancellationToken);  
    }
}