using MemQuran.Core.Contracts;
using MemQuran.Infrastructure.Factories;
using MemQuran.Core.Models;
using MemQuran.Infrastructure.Caching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CachingExtensions
{
    public class CachingConfiguration
    {
        public CacheType CacheType { get; set; }
        public TimeSpan CacheDurationTimeSpan { get; set; }
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
        services.AddSingleton<ICachingProvider, HybridCachingProvider>();
        services.AddSingleton<ICachingProvider, DistributedCachingProvider>();

        var fusionCacheBuilder = services.AddFusionCache().WithSerializer(new FusionCacheSystemTextJsonSerializer());
        var fusionCacheOptions = new FusionCacheOptions();
        var fusionCacheEntryOptions = new FusionCacheEntryOptions();

        fusionCacheOptions.FailSafeActivationLogLevel = LogLevel.Debug;
        fusionCacheOptions.SerializationErrorsLogLevel = LogLevel.Warning;
        fusionCacheOptions.FactorySyntheticTimeoutsLogLevel = LogLevel.Debug;
        fusionCacheOptions.FactoryErrorsLogLevel = LogLevel.Error;
        // fusionCacheOptions.CacheKeyPrefix = "memquranapi:";
        fusionCacheEntryOptions.Duration = config.CacheDurationTimeSpan;
        fusionCacheEntryOptions.EagerRefreshThreshold = 0.9f;
        fusionCacheEntryOptions.FactorySoftTimeout = TimeSpan.FromMilliseconds(100);
        fusionCacheEntryOptions.FactoryHardTimeout = TimeSpan.FromMilliseconds(1500);

        if (config.CacheType is CacheType.Hybrid or CacheType.Distributed)
        {
            fusionCacheOptions.DistributedCacheCircuitBreakerDuration = TimeSpan.FromSeconds(2);
            fusionCacheOptions.DistributedCacheSyntheticTimeoutsLogLevel = LogLevel.Debug;
            fusionCacheOptions.DistributedCacheErrorsLogLevel = LogLevel.Error;
            fusionCacheEntryOptions.IsFailSafeEnabled = true;
            fusionCacheEntryOptions.FailSafeMaxDuration = config.CacheDurationTimeSpan;
            fusionCacheEntryOptions.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
            fusionCacheEntryOptions.DistributedCacheSoftTimeout = TimeSpan.FromSeconds(1);
            fusionCacheEntryOptions.DistributedCacheHardTimeout = TimeSpan.FromSeconds(2);
            fusionCacheEntryOptions.AllowBackgroundDistributedCacheOperations = true;
            fusionCacheEntryOptions.JitterMaxDuration = TimeSpan.FromSeconds(2);

            fusionCacheBuilder
                .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = config.RedisConnectionString, InstanceName = "memquranapi:" }))
                .WithBackplane(new RedisBackplane(new RedisBackplaneOptions { Configuration = config.RedisConnectionString }));
        }

        if (config.CacheType == CacheType.Distributed)
        {
            fusionCacheEntryOptions.SkipMemoryCacheRead = true;
            fusionCacheEntryOptions.SkipMemoryCacheWrite = true;
        }

        fusionCacheBuilder.WithOptions(fusionCacheOptions);
        fusionCacheBuilder.WithDefaultEntryOptions(fusionCacheEntryOptions);

        return services;
    }
}