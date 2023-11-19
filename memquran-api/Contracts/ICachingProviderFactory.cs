using QuranApi.Models;

namespace QuranApi.Contracts;

public interface ICachingProviderFactory
{
    ICachingProvider GetCachingProvider(CacheType cacheType);
}