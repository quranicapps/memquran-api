using MemQuran.Api.Caching.Worker.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZiggyCreatures.Caching.Fusion;

namespace MemQuran.Api.Caching.Worker;

public class EvictCacheFunction(IFusionCache cache, ILogger<EvictCacheFunction> logger)
{
    [Function("EvictCacheItem")]
    public async Task<IActionResult> EvictCacheItem([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [Microsoft.Azure.Functions.Worker.Http.FromBody] EvictCacheItemRequest request)
    {
        await cache.RemoveAsync(request.CacheKey);

        return new OkObjectResult($"{nameof(EvictCacheItem)} - Cache Item Evicted: {request.CacheKey}");
    }
    
    [Function("EvictCacheAll")]
    public async Task<IActionResult> EvictCacheAll([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        await cache.ClearAsync(false);
        
        return new OkObjectResult($"{nameof(EvictCacheAll)} - All Cache Items Evicted");

    }
}