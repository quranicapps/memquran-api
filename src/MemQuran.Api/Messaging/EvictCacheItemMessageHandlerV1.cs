using MemQuran.Api.Extensions;
using Topica.Contracts;
using Topica.Messages;
using ZiggyCreatures.Caching.Fusion;

namespace MemQuran.Api.Messaging;

public class EvictCacheItemMessageHandlerV1(IFusionCache cache, ILogger<EvictCacheItemMessageHandlerV1> logger) : IHandler<EvictCacheItemMessageV1>
{
    public async Task<bool> HandleAsync(EvictCacheItemMessageV1 source, Dictionary<string, string>? properties)
    {
        using var activity = ActivityExtensions.GetActivityFromMessageProperties(nameof(EvictCacheItemMessageV1), $"OT:Process-{nameof(EvictCacheItemMessageV1)}-Start", properties, source, new Dictionary<string, string>
        {
            { "CacheKey", source.CacheKey },
            { "MessageType", nameof(EvictCacheItemMessageV1) },
            { "Handler", nameof(EvictCacheItemMessageHandlerV1) }
        });

        logger.LogInformation("Handle: {Name} for cache key: {CacheKey}", nameof(EvictCacheItemMessageV1), source.CacheKey);
        
        await cache.RemoveAsync(source.CacheKey);
        
        return await Task.FromResult(true);
    }

    public bool ValidateMessage(EvictCacheItemMessageV1 message)
    {
        var isValid = !string.IsNullOrWhiteSpace(message.CacheKey);
        return isValid;
    }
}

public class EvictCacheItemMessageV1 : BaseMessage
{
    public string CacheKey { get; init; } = null!;
}