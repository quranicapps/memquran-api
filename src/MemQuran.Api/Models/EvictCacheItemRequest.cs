namespace MemQuran.Api.Models;

public class EvictCacheItemRequest : BaseRequest
{
    public required string CacheKey { get; set; }
}