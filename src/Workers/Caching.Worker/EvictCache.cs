using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caching.Worker;

public class EvictCache(ILogger<EvictCache> logger)
{
    [Function("EvictCache")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request");

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}