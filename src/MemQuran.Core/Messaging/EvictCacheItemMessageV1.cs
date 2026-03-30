using Topica.Messages;

namespace MemQuran.Core.Messaging;

public class EvictCacheItemMessageV1 : BaseMessage
{
    public string CacheKey { get; init; } = null!;
}