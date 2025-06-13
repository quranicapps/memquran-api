using MemQuran.Api.Models;

namespace MemQuran.Api.Contracts;

public interface ICachingProvider
{
    CacheType CacheType { get; }
    
    Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, byte[] value, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default);
}