using MemQuran.Api.Extensions;
using Topica.Contracts;
using Topica.Messages;
using ZiggyCreatures.Caching.Fusion;

namespace MemQuran.Api.Messaging;

public class EvictCacheAllMessageHandlerV1(IFusionCache cache, ILogger<EvictCacheAllMessageHandlerV1> logger) : IHandler<EvictCacheAllMessageV1>
{
    public async Task<bool> HandleAsync(EvictCacheAllMessageV1 source, Dictionary<string, string>? properties)
    {
        using var activity = ActivityExtensions.GetActivityFromMessageProperties(nameof(EvictCacheAllMessageV1), $"OT:Process-{nameof(EvictCacheAllMessageV1)}-Start", properties, source, new Dictionary<string, string>
        {
            { "MessageType", nameof(EvictCacheAllMessageV1) },
            { "Handler", nameof(EvictCacheAllMessageHandlerV1) }
        });

        logger.LogInformation("Handle: {Name}", nameof(EvictCacheAllMessageV1));
        
        await cache.ClearAsync(false);
        
        return await Task.FromResult(true);
    }

    public bool ValidateMessage(EvictCacheAllMessageV1 message)
    {
        return true;
    }
}

public class EvictCacheAllMessageV1 : BaseMessage;