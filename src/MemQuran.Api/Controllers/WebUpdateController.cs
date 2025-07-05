using System.Diagnostics;
using MemQuran.Api.Messaging;
using MemQuran.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Topica.Contracts;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WebUpdateController([FromKeyedServices("WebUpdateProducer")] IProducer producer, ILogger<WebUpdateController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> EvictCacheItem(UpdateCacheItemRequest updateCacheItemRequest, CancellationToken cancellationToken)
    {
        var message = new EvictCacheItemMessageV1
        {
            Id = Guid.NewGuid(),
            CacheKey = Guid.NewGuid().ToString(),
            ConversationId = Guid.NewGuid(),
            EventId = 1,
            EventName = "evict.cache.item.v1",
            Type = nameof(EvictCacheItemMessageV1),
            RaisingComponent = $"{typeof(WebUpdateController).FullName}.{nameof(EvictCacheItem)}",
            SourceIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            TimeStampUtc = DateTimeOffset.UtcNow.DateTime,
            Version = "V1",
            MessageGroupId = Guid.NewGuid().ToString()
        };

        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var traceState = Activity.Current?.TraceStateString;

        var attributes = new Dictionary<string, string>
        {
            { "traceparent", traceId },
            { "tracestate", traceState ?? "unknown" }
        };

        await producer.ProduceAsync(message, attributes, cancellationToken);

        logger.LogInformation("Received request to update cache item: {CacheKey}", updateCacheItemRequest.CacheKey);

        return Accepted();
    }
}