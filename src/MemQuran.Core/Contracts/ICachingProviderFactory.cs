using MemQuran.Core.Models;

namespace MemQuran.Core.Contracts;

public interface ICachingProviderFactory
{
    ICachingProvider GetCachingProvider(CacheType cacheType);
}