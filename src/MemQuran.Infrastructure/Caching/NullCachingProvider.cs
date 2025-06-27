using MemQuran.Core.Contracts;
using MemQuran.Core.Models;

namespace MemQuran.Infrastructure.Caching;

public class NullCachingProvider : ICachingProvider
{
    public CacheType Name => CacheType.None;
    
    public async Task<string> GetOrCreateStringAsync(string key, Func<CancellationToken, Task<string?>> func, CancellationToken cancellationToken = default)
    {
        var text = await func(cancellationToken);
        
        if (text is null)throw new InvalidOperationException("CDN returned null for key: " + key);
        
        return text;
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}