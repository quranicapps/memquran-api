using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Models;

namespace QuranApi.Contracts;

public class MemoryCachingProvider : ICachingProvider
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<MemoryCachingProvider> _logger;

    public MemoryCachingProvider(IDistributedCache distributedCache, ILogger<MemoryCachingProvider> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }
    
    public CacheType CacheType => CacheType.Memory;
    
    public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _distributedCache.GetAsync(key, token: cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var item = await _distributedCache.GetStringAsync(key, token: cancellationToken).ConfigureAwait(false);

        if (item is null)
        {
            _logger.LogInformation("***** Cache miss for {Key}", key);
        }
        
        return item;
    }

    public async Task SetAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        await _distributedCache.SetAsync(key, value, cancellationToken);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await _distributedCache.SetStringAsync(key, value, cancellationToken);
    }
}