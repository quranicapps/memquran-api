using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MemQuran.Infrastructure.Caching;

public class HybridCachingProvider(HybridCache hybridCache, ILogger<MemoryCachingProvider> logger) : ICachingProvider
{
    public CacheType Name => CacheType.Hybrid;

    public async Task<string?> GetOrCreateStringAsync(string key, Func<CancellationToken, Task<string?>> func, CancellationToken cancellationToken = default)
    {
        return await hybridCache.GetOrCreateAsync<string?>
        (
            key, 
            async ct => await func(ct), 
            cancellationToken: cancellationToken
        );
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await hybridCache.SetAsync(key, value, cancellationToken: cancellationToken);
    }
}