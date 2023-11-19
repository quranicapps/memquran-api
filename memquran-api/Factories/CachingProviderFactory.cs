using QuranApi.Models;

namespace QuranApi.Contracts;

public class CachingProviderFactory : ICachingProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CachingProviderFactory> _logger;

    public CachingProviderFactory(IServiceProvider serviceProvider, ILogger<CachingProviderFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public ICachingProvider GetCachingProvider(CacheType cacheType)
    {
        var providers = _serviceProvider.GetServices<ICachingProvider>();
        
        var cachingProvider = cacheType switch
        {
            CacheType.None => providers.FirstOrDefault(x => x.CacheType == CacheType.None),
            CacheType.Memory => providers.FirstOrDefault(x => x.CacheType == CacheType.Memory),
            CacheType.Redis => providers.FirstOrDefault(x => x.CacheType == CacheType.Redis),
            _ => throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null)
        };

        _logger.LogInformation("{Name} used as caching provider", cachingProvider?.GetType().Name);

        return cachingProvider;
    }
}