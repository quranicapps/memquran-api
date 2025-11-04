using System.Diagnostics;
using System.Threading.Channels;
using MemQuran.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WebUpdateController(
    Channel<EvictCacheItemRequest> evictCacheItemChannel,
    Channel<EvictCacheAllRequest> evictCacheAllChannel,
    ILogger<WebUpdateController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> EvictCacheItem(EvictCacheItemRequest request, CancellationToken cancellationToken)
    {
        request.SourceIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        request.TraceId = Activity.Current?.Id;
        request.TraceState = Activity.Current?.TraceStateString;
        request.Baggage = Activity.Current?.Baggage;

        await evictCacheItemChannel.Writer.WaitToWriteAsync(cancellationToken);
        evictCacheItemChannel.Writer.TryWrite(request);

        return Accepted();
    }

    [HttpPost]
    public async Task<IActionResult> EvictCacheAll(CancellationToken cancellationToken)
    {
        var request = new EvictCacheAllRequest
        {
            SourceIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            TraceId = Activity.Current?.Id,
            TraceState = Activity.Current?.TraceStateString,
            Baggage = Activity.Current?.Baggage
        };

        await evictCacheAllChannel.Writer.WaitToWriteAsync(cancellationToken);
        evictCacheAllChannel.Writer.TryWrite(request);

        return Accepted();
    }
}