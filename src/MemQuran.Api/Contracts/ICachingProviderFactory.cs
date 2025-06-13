using MemQuran.Api.Models;

namespace MemQuran.Api.Contracts;

public interface ICachingProviderFactory
{
    ICachingProvider GetCachingProvider(CacheType cacheType);
}