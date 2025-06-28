using MemQuran.Core.Models;

namespace MemQuran.Core.Contracts;

public interface ICachingProvider
{
    CacheType Name { get; }
    
    Task<string?> GetOrCreateStringAsync(string key, Func<CancellationToken, Task<string?>> func, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default);
}