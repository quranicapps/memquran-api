using System.Diagnostics;
using System.Threading.Channels;
using MemQuran.Api.Messaging;
using MemQuran.Api.Models;
using Topica.Contracts;

namespace MemQuran.Api.Workers;

public class WebUpdateProducerWorker(
    Channel<EvictCacheItemRequest> evictCacheItemChannel,
    [FromKeyedServices("WebUpdateProducer")]
    IProducer producer,
    ILogger<WebUpdateProducerWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in evictCacheItemChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var message = new EvictCacheItemMessageV1
            {
                Id = Guid.NewGuid(),
                CacheKey = request.CacheKey,
                ConversationId = Guid.NewGuid(),
                EventId = 1,
                EventName = "evict.cache.item.v1",
                Type = nameof(EvictCacheItemMessageV1),
                RaisingComponent = $"{typeof(WebUpdateProducerWorker).FullName}.{nameof(ExecuteAsync)}",
                SourceIp = request.SourceIpAddress,
                TimeStampUtc = DateTimeOffset.UtcNow.DateTime,
                Version = "V1",
                MessageGroupId = Guid.NewGuid().ToString()
            };

            var traceId = request.TraceId;
            var traceState = request.TraceState ?? Activity.Current?.TraceStateString;

            var attributes = new Dictionary<string, string>
            {
                { "traceparent", traceId },
                { "tracestate", traceState ?? "unknown" }
            };

            await producer.ProduceAsync(message, attributes, stoppingToken);
        }
    }
}