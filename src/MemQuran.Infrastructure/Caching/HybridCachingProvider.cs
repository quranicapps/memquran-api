using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace MemQuran.Infrastructure.Caching;

public class HybridCachingProvider(IFusionCache cache, ILogger<MemoryCachingProvider> logger) : ICachingProvider
{
    public CacheType Name => CacheType.Hybrid;

    public async Task<string?> GetOrCreateStringAsync(string key, Func<CancellationToken, Task<string?>> func, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrSetAsync<string?>(key, async ct =>
        {
            logger.LogInformation("***** ({Name}) Cache miss for {Key}", nameof(HybridCachingProvider), key);
            return await func(ct);
        }, token: cancellationToken);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await cache.SetAsync(key, value, token: cancellationToken);
    }
}