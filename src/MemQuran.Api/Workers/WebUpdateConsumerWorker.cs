using Topica.Contracts;

namespace MemQuran.Api.Workers;

public class WebUpdateConsumerWorker([FromKeyedServices("WebUpdateConsumer")] IConsumer consumer, ILogger<WebUpdateConsumerWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.ConsumeAsync(stoppingToken);
    }
}