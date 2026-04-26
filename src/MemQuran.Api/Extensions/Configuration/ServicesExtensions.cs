using System.Net.Http.Headers;
using System.Threading.Channels;
using MemQuran.Api.Clients.JsDelivr;
using MemQuran.Api.Clients.Local;
using MemQuran.Api.Services;
using MemQuran.Core.Contracts;
using MemQuran.Infrastructure.Factories;
using MemQuran.Infrastructure.Services;
using MemQuran.Api.Models;
using MemQuran.Infrastructure.Messaging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public class ServicesOptions
    {
        public string JsDelivrServiceBaseUrl { get; set; } = null!;
        public TimeSpan JsDelivrServiceDefaultTimeout { get; set; }
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddServices(this IServiceCollection services, Action<ServicesOptions> optionsFactory)
    {
        var options = new ServicesOptions();
        optionsFactory(options);

        services.AddSingleton<IHashingService, HashingService>();
        services.AddSingleton<IStaticFileService, StaticFileService>();

        services.AddSingleton<ICdnClient, LocalFileClient>();
        services.AddHttpClient<ICdnClient, JsDelivrClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(options.JsDelivrServiceBaseUrl);
                httpClient.Timeout = options.JsDelivrServiceDefaultTimeout;
            })
            .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
        services.AddSingleton<ICdnClientFactory, CdnClientFactory>();

        // Add .NET Channels
        var messagingChannel = new MessagingChannel();
        services.AddSingleton<Channel<EvictCacheItemRequest>>(_ => messagingChannel.CreateBounded<EvictCacheItemRequest>());
        services.AddSingleton<Channel<EvictCacheAllRequest>>(_ => messagingChannel.CreateBounded<EvictCacheAllRequest>());

        return services;
    }
}