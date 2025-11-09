using Azure.Identity;
using FluentValidation;
using MemQuran.Api.Caching.Worker.Settings;
using MemQuran.Api.Caching.Worker.Validators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

if (builder.Environment.IsStaging() || builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["AzureKeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = builder.Configuration["AzureUserManagedIdentityClientId"]
        })
    );
}

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
if (redisSettings == null) throw new InvalidOperationException($"{nameof(RedisSettings)} is not configured. Please check your appsettings.json or environment variables.");
new RedisSettingsValidator().ValidateAndThrow(redisSettings);
builder.Services.AddSingleton(redisSettings);

builder.Services.AddFusionCache()
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = redisSettings.ConnectionString, InstanceName = "memquranapi:" }))
    .WithBackplane(new RedisBackplane(new RedisBackplaneOptions { Configuration = redisSettings.ConnectionString }))
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        SkipMemoryCacheRead = true,
        SkipMemoryCacheWrite = true
    });

builder.Build().Run();