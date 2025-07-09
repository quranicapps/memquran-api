using System.Threading.Channels;
using MemQuran.Api.Clients.JsDelivr;
using MemQuran.Api.Clients.Local;
using MemQuran.Api.Services;
using MemQuran.Core.Contracts;
using MemQuran.Infrastructure.Factories;
using MemQuran.Infrastructure.Services;
using MemQuran.Api.Configuration.ApiServices;
using MemQuran.Api.Models;
using MemQuran.Infrastructure.Messaging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ApiServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, Action<ApiConfiguration> configuration)
    {
        var config = new ApiConfiguration();
        configuration(config);

        services.AddSingleton<IHashingService, HashingService>();
        services.AddSingleton<IStaticFileService, StaticFileService>();
        services.AddScoped<JsDelivrDelegatingHandler>();
        services.AddSingleton<ICdnClient, LocalFileClient>();
        services.AddHttpClient<ICdnClient, JsDelivrClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(config.JsDelivrServiceBaseUrl);
                httpClient.Timeout = config.JsDelivrServiceDefaultTimeout;
            })
            .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
        services.AddSingleton<ICdnClientFactory, CdnClientFactory>();
        
        // Add .NET Channels
        var messagingChannel = new MessagingChannel();
        services.AddSingleton<Channel<EvictCacheItemRequest>>(_ => messagingChannel.CreateBounded<EvictCacheItemRequest>());

        return services;
    }
}