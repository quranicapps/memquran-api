using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemQuran.Infrastructure.Factories;

public class CachingProviderFactory(IServiceProvider serviceProvider, ILogger<CachingProviderFactory> logger) : ICachingProviderFactory
{
    public ICachingProvider GetCachingProvider(CacheType cacheType)
    {
        var providers = serviceProvider.GetServices<ICachingProvider>();
        
        var cachingProvider = cacheType switch
        {
            CacheType.None => providers.FirstOrDefault(x => x.CacheType == CacheType.None),
            CacheType.Memory => providers.FirstOrDefault(x => x.CacheType == CacheType.Memory),
            CacheType.Redis => providers.FirstOrDefault(x => x.CacheType == CacheType.Redis),
            _ => throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null)
        };

        logger.LogInformation("{Name} used as caching provider", cachingProvider?.GetType().Name);

        return cachingProvider;
    }
}