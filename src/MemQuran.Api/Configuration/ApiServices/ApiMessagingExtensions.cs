using System.Reflection;
using Topica.Aws.Contracts;
using Topica.Contracts;

namespace MemQuran.Api.Configuration.ApiServices;

public static class ApiMessagingExtensions
{
    public static IServiceCollection AddMessagingServices(this IServiceCollection services, Action<ApiConfiguration> configuration, CancellationToken cancellationToken)
    {
        var config = new ApiConfiguration();
        configuration(config);

        // AWS Messaging Configuration
        services.AddAwsTopica(c =>
        {
            c.ProfileName = config.AwsHostSettings.ProfileName;
            c.AccessKey = config.AwsHostSettings.AccessKey;
            c.SecretKey = config.AwsHostSettings.SecretKey;
            c.ServiceUrl = config.AwsHostSettings.ServiceUrl;
            c.RegionEndpoint = config.AwsHostSettings.RegionEndpoint;
        }, Assembly.GetExecutingAssembly());

        var builder = services.BuildServiceProvider().GetRequiredService<IAwsQueueCreationBuilder>()
            .WithWorkerName(config.AwsConsumerSettings.WebUpdateQueueSettings.WorkerName)
            .WithQueueName(config.AwsConsumerSettings.WebUpdateQueueSettings.Source)
            .WithErrorQueueSettings(
                config.AwsConsumerSettings.WebUpdateQueueSettings.BuildWithErrorQueue,
                config.AwsConsumerSettings.WebUpdateQueueSettings.ErrorQueueMaxReceiveCount
            )
            .WithFifoSettings(
                config.AwsConsumerSettings.WebUpdateQueueSettings.IsFifoQueue,
                config.AwsConsumerSettings.WebUpdateQueueSettings.IsFifoContentBasedDeduplication
            )
            .WithTemporalSettings(
                config.AwsConsumerSettings.WebUpdateQueueSettings.MessageVisibilityTimeoutSeconds,
                config.AwsConsumerSettings.WebUpdateQueueSettings.QueueMessageDelaySeconds,
                config.AwsConsumerSettings.WebUpdateQueueSettings.QueueMessageRetentionPeriodSeconds,
                config.AwsConsumerSettings.WebUpdateQueueSettings.QueueReceiveMessageWaitTimeSeconds
            )
            .WithQueueSettings(config.AwsConsumerSettings.WebUpdateQueueSettings.QueueMaximumMessageSize)
            .WithConsumeSettings(
                config.AwsConsumerSettings.WebUpdateQueueSettings.NumberOfInstances,
                config.AwsConsumerSettings.WebUpdateQueueSettings.QueueReceiveMaximumNumberOfMessages
            );

        services.AddKeyedSingleton<IConsumer>("WebUpdateConsumer", (_, _) => builder.BuildConsumerAsync(cancellationToken).Result);
        services.AddKeyedSingleton<IProducer>("WebUpdateProducer", (_, _) => builder.BuildProducerAsync(cancellationToken).Result);

        return services;
    }
}