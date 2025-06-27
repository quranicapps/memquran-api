using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MemQuran.Infrastructure.Caching;

public class MemoryCachingProvider(IDistributedCache distributedCache, ILogger<MemoryCachingProvider> logger) : ICachingProvider
{
    public CacheType Name => CacheType.Memory;
    
    public async Task<string> GetOrCreateStringAsync(string key, Func<CancellationToken, Task<string?>> func, CancellationToken cancellationToken = default)
    {
        var text = await distributedCache.GetStringAsync(key, cancellationToken);

        if (text is not null) return text;
        
        logger.LogInformation("***** Cache miss for {Key}", key);

        var item = await func(cancellationToken);
        
        if (item is null) throw new InvalidOperationException("CDN returned null for key: " + key);
        
        await distributedCache.SetStringAsync(key, item, cancellationToken);

        return item;
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetStringAsync(key, value, cancellationToken);
    }
}