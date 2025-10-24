using MemQuran.Core.Contracts;
using MemQuran.Infrastructure.Factories;
using MemQuran.Core.Models;
using MemQuran.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CachingExtensions
{
    public class CachingConfiguration
    {
        public CacheType CacheType { get; set; }
        public string RedisConnectionString { get; set; } = null!;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddCachingServices(this IServiceCollection services, Action<CachingConfiguration> configuration)
    {
        var config = new CachingConfiguration();
        configuration(config);

        services.AddSingleton<ICachingProviderFactory, CachingProviderFactory>();
        services.AddSingleton<ICachingProvider, NullCachingProvider>();
        services.AddSingleton<ICachingProvider, MemoryCachingProvider>();

        if (config.CacheType == CacheType.Hybrid)
        {
            // Caching - Hybrid Cache will use both local in-memory cache and distributed cache (any IDistributedCache implementation, i.e. AddStackExchangeRedisCache)
            services.AddStackExchangeRedisCache(options =>
            {
                // Don't use when using ConfigurationOptions, otherwise it will throw an exception
                // options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "memquranapi:";
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints = { config.RedisConnectionString },
                    AsyncTimeout = 5000, // 5 seconds
                    SyncTimeout = 5000, // 5 seconds
                    AbortOnConnectFail = false, // Do not throw an exception if the connection fails
                    ConnectTimeout = 5000, // 5 seconds
                };
            });
            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromDays(7), // Distributed cache expiration
                    LocalCacheExpiration = TimeSpan.FromDays(7) // Local cache expiration
                };
                options.MaximumPayloadBytes = 1024 * 1024 * 100; // 100 MB
            });
            services.AddSingleton<ICachingProvider, HybridCachingProvider>();
        }
        else
        {
            // Caching - Memory Cache will use in-memory cache only
            services.AddDistributedMemoryCache(options => { options.SizeLimit = long.MaxValue; });
        }

        return services;
    }
}