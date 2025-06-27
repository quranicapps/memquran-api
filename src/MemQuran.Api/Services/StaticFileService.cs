using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;

namespace MemQuran.Api.Services;

public class StaticFileService(
    ICachingProviderFactory cachingProviderFactory, 
    ICdnClientFactory cdnClientFactory, 
    ContentDeliverySettings contentDeliverySettings, 
    ILogger<StaticFileService> logger)
    : IStaticFileService
{
    private readonly ICachingProvider _cachingProvider = cachingProviderFactory.GetCachingProvider(contentDeliverySettings.CachingSettings.CacheType);
    private readonly ICdnClient _cdnClient = cdnClientFactory.Create(contentDeliverySettings.Type);

    public async Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var cacheKey = Path.GetFileName(filePath);
        
        var text = await _cachingProvider.GetOrCreateStringAsync
        (
            cacheKey, 
            ct => _cdnClient.GetFileContentStringAsync(filePath, ct), 
            cancellationToken
        );
        
        return text;
    }

    public async Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await _cdnClient.GetFileContentBytesAsync(filePath, cancellationToken);  
    }
}