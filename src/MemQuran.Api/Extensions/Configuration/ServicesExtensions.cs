using System.Net.Http.Headers;
using System.Threading.Channels;
using MemQuran.Api.Clients.BetterStack;
using MemQuran.Api.Clients.JsDelivr;
using MemQuran.Api.Clients.Local;
using MemQuran.Api.Services;
using MemQuran.Core.Contracts;
using MemQuran.Infrastructure.Factories;
using MemQuran.Infrastructure.Services;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using MemQuran.Infrastructure.Messaging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public class ServicesOptions
    {
        public ClientsSettings ClientSettings { get; set; } = null!;
        public BetterStackSettings BetterStackSettings { get; set; } = null!;
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
                httpClient.BaseAddress = new Uri(options.ClientSettings.JsDelivrService.BaseUrl);
                httpClient.Timeout = options.ClientSettings.JsDelivrService.DefaultTimeout;
            })
            .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
        services.AddSingleton<ICdnClientFactory, CdnClientFactory>();

        services.AddHttpClient<ITelemetryClient, BetterStackTelemetryClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(options.BetterStackSettings.IngestBaseUrl);
                httpClient.Timeout = options.BetterStackSettings.DefaultTimeout;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.BetterStackSettings.BearerToken}");
            })
            .AddHttpMessageHandler(() => new BetterStackDelegatingHandler());

        // Add .NET Channels
        var messagingChannel = new MessagingChannel();
        services.AddSingleton<Channel<EvictCacheItemRequest>>(_ => messagingChannel.CreateBounded<EvictCacheItemRequest>());
        services.AddSingleton<Channel<EvictCacheAllRequest>>(_ => messagingChannel.CreateBounded<EvictCacheAllRequest>());

        return services;
    }
}