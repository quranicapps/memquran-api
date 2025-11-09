using System.Reflection;
using MemQuran.Api.Settings;
using Topica.Aws.Contracts;
using Topica.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingExtensions
{
    public class MessagingOptions
    {
        public AwsHostSettings AwsHostSettings { get; set; } = null!;
        public AwsTopicSettings AwsTopicSettings { get; set; } = null!;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static async Task<IServiceCollection> AddMessagingServices(this IServiceCollection services, Action<MessagingOptions> optionsFactory, CancellationToken cancellationToken)
    {
        var options = new MessagingOptions();
        optionsFactory(options);

        // AWS Messaging Configuration
        services.AddAwsTopica(c =>
        {
            c.ProfileName = options.AwsHostSettings.ProfileName;
            c.AccessKey = options.AwsHostSettings.AccessKey;
            c.SecretKey = options.AwsHostSettings.SecretKey;
            c.ServiceUrl = options.AwsHostSettings.ServiceUrl;
            c.RegionEndpoint = options.AwsHostSettings.RegionEndpoint;
        }, Assembly.GetExecutingAssembly());

        var builder = services.BuildServiceProvider().GetRequiredService<IAwsTopicCreationBuilder>()
            .WithWorkerName(options.AwsTopicSettings.WebUpdateTopicSettings.WorkerName)
            .WithTopicName(options.AwsTopicSettings.WebUpdateTopicSettings.Source)
            .WithSubscribedQueues(options.AwsTopicSettings.WebUpdateTopicSettings.WithSubscribedQueues)
            .WithQueueToSubscribeTo(options.AwsTopicSettings.WebUpdateTopicSettings.SubscribeToSource)
            .WithErrorQueueSettings(
                options.AwsTopicSettings.WebUpdateTopicSettings.BuildWithErrorQueue,
                options.AwsTopicSettings.WebUpdateTopicSettings.ErrorQueueMaxReceiveCount
            )
            .WithFifoSettings(
                options.AwsTopicSettings.WebUpdateTopicSettings.IsFifoQueue,
                options.AwsTopicSettings.WebUpdateTopicSettings.IsFifoContentBasedDeduplication
            )
            .WithTemporalSettings(
                options.AwsTopicSettings.WebUpdateTopicSettings.MessageVisibilityTimeoutSeconds,
                options.AwsTopicSettings.WebUpdateTopicSettings.QueueMessageDelaySeconds,
                options.AwsTopicSettings.WebUpdateTopicSettings.QueueMessageRetentionPeriodSeconds,
                options.AwsTopicSettings.WebUpdateTopicSettings.QueueReceiveMessageWaitTimeSeconds
            )
            .WithQueueSettings(options.AwsTopicSettings.WebUpdateTopicSettings.QueueMaximumMessageSize)
            .WithConsumeSettings(
                options.AwsTopicSettings.WebUpdateTopicSettings.NumberOfInstances,
                options.AwsTopicSettings.WebUpdateTopicSettings.QueueReceiveMaximumNumberOfMessages
            );

        var consumer = await builder.BuildConsumerAsync(cancellationToken);
        var producer = await builder.BuildProducerAsync(cancellationToken);
        services.AddKeyedSingleton<IConsumer>("WebUpdateConsumer",  (_, _) => consumer);
        services.AddKeyedSingleton<IProducer>("WebUpdateProducer", (_, _) => producer);

        return services;
    }
}