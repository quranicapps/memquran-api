using System.Reflection;
using MemQuran.Api.Settings.Messaging;
using Topica.Aws.Contracts;
using Topica.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingExtensions
{
    public class MessagingConfiguration
    {
        public AwsHostSettings AwsHostSettings { get; set; } = null!;
        public AwsTopicSettings AwsTopicSettings { get; set; } = null!;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static async Task<IServiceCollection> AddMessagingServices(this IServiceCollection services, Action<MessagingConfiguration> configuration, CancellationToken cancellationToken)
    {
        var config = new MessagingConfiguration();
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

        var builder = services.BuildServiceProvider().GetRequiredService<IAwsTopicCreationBuilder>()
            .WithWorkerName(config.AwsTopicSettings.WebUpdateTopicSettings.WorkerName)
            .WithTopicName(config.AwsTopicSettings.WebUpdateTopicSettings.Source)
            .WithSubscribedQueues(config.AwsTopicSettings.WebUpdateTopicSettings.WithSubscribedQueues)
            .WithQueueToSubscribeTo(config.AwsTopicSettings.WebUpdateTopicSettings.SubscribeToSource)
            .WithErrorQueueSettings(
                config.AwsTopicSettings.WebUpdateTopicSettings.BuildWithErrorQueue,
                config.AwsTopicSettings.WebUpdateTopicSettings.ErrorQueueMaxReceiveCount
            )
            .WithFifoSettings(
                config.AwsTopicSettings.WebUpdateTopicSettings.IsFifoQueue,
                config.AwsTopicSettings.WebUpdateTopicSettings.IsFifoContentBasedDeduplication
            )
            .WithTemporalSettings(
                config.AwsTopicSettings.WebUpdateTopicSettings.MessageVisibilityTimeoutSeconds,
                config.AwsTopicSettings.WebUpdateTopicSettings.QueueMessageDelaySeconds,
                config.AwsTopicSettings.WebUpdateTopicSettings.QueueMessageRetentionPeriodSeconds,
                config.AwsTopicSettings.WebUpdateTopicSettings.QueueReceiveMessageWaitTimeSeconds
            )
            .WithQueueSettings(config.AwsTopicSettings.WebUpdateTopicSettings.QueueMaximumMessageSize)
            .WithConsumeSettings(
                config.AwsTopicSettings.WebUpdateTopicSettings.NumberOfInstances,
                config.AwsTopicSettings.WebUpdateTopicSettings.QueueReceiveMaximumNumberOfMessages
            );

        var consumer = await builder.BuildConsumerAsync(cancellationToken);
        var producer = await builder.BuildProducerAsync(cancellationToken);
        services.AddKeyedSingleton<IConsumer>("WebUpdateConsumer",  (_, _) => consumer);
        services.AddKeyedSingleton<IProducer>("WebUpdateProducer", (_, _) => producer);

        return services;
    }
}