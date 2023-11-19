using QuranApi.Models;

namespace QuranApi.Contracts;

public class NullCachingProvider : ICachingProvider
{
    public CacheType CacheType => CacheType.None;
    
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return default;
    }

    public Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult((string)null);
    }

    public async Task SetAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}