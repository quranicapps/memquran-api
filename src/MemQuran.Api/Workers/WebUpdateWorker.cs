using Topica.Contracts;

namespace MemQuran.Api.Workers;

public class WebUpdateWorker([FromKeyedServices("WebUpdateConsumer")] IConsumer consumer, ILogger<WebUpdateWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.ConsumeAsync(stoppingToken);
    }
}