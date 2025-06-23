using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MemQuran.Infrastructure.Caching;

public class MemoryCachingProvider(IDistributedCache distributedCache, ILogger<MemoryCachingProvider> logger) : ICachingProvider
{
    public CacheType Name => CacheType.Memory;
    
    public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await distributedCache.GetAsync(key, token: cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var item = await distributedCache.GetStringAsync(key, token: cancellationToken).ConfigureAwait(false);

        if (item is null)
        {
            logger.LogInformation("***** Cache miss for {Key}", key);
        }
        
        return item;
    }

    public async Task SetAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetAsync(key, value, cancellationToken);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetStringAsync(key, value, cancellationToken);
    }
}