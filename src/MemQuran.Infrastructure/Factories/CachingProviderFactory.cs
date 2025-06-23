using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemQuran.Infrastructure.Factories;

public class CachingProviderFactory(IServiceProvider serviceProvider, ILogger<CachingProviderFactory> logger) : ICachingProviderFactory
{
    public ICachingProvider GetCachingProvider(CacheType cacheType)
    {
        var cachingProviders = serviceProvider.GetServices<ICachingProvider>().ToList();
        return cachingProviders.FirstOrDefault(x => x.Name == cacheType) ?? throw new InvalidOperationException($"No caching provider found for cache type {cacheType}. Available types: {string.Join(", ", cachingProviders.Select(c => c.Name))}");
    }
}