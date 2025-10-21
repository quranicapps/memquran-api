using System.Diagnostics;
using System.Threading.Channels;
using MemQuran.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MemQuran.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WebUpdateController(Channel<EvictCacheItemRequest> evictCacheItemChannel, ILogger<WebUpdateController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> EvictCacheItem(EvictCacheItemRequest request, CancellationToken cancellationToken)
    {
        request.SourceIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        request.TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        request.TraceState = Activity.Current?.TraceStateString;
        
        await evictCacheItemChannel.Writer.WaitToWriteAsync(cancellationToken);
        evictCacheItemChannel.Writer.TryWrite(request);

        return Accepted();
    }
}