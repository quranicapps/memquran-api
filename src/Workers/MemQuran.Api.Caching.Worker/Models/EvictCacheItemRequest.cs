namespace MemQuran.Api.Caching.Worker.Models;

public class EvictCacheItemRequest
{
    public required string CacheKey { get; set; }
}