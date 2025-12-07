using Azure.Identity;
using FluentValidation;
using MemQuran.Api.Messaging;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using MemQuran.Api.Validators;
using MemQuran.Api.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

////////////////////////////
// Configure Services

// Startup Cancellation Token, cancelled when the app.Lifetime.ApplicationStopping
var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

// Configuration

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

var identityProviderSettings = builder.Configuration.GetSection(IdentityProviderSettings.SectionName).Get<IdentityProviderSettings>();
if (identityProviderSettings == null) throw new InvalidOperationException($"{nameof(IdentityProviderSettings)} is not configured. Please check your appsettings.json or environment variables.");

var healthCheckSettings = builder.Configuration.GetSection(HealthCheckSettings.SectionName).Get<HealthCheckSettings>() ?? throw new Exception("Could not bind the HealthCheck Settings, please check configuration");
if (healthCheckSettings == null) throw new InvalidOperationException($"{nameof(HealthCheckSettings)} is not configured. Please check your appsettings.json or environment variables.");
builder.Services.AddSingleton(healthCheckSettings);

var contentDeliverySettings = builder.Configuration.GetSection(ContentDeliverySettings.SectionName).Get<ContentDeliverySettings>();
if (contentDeliverySettings == null) throw new Exception("Could not bind the Content Delivery Settings, please check configuration");
builder.Services.AddSingleton(contentDeliverySettings);

var clientsSettings = builder.Configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
if (clientsSettings == null) throw new Exception("Could not bind the Clients Settings, please check configuration");
builder.Services.AddSingleton(clientsSettings);

var pingSettings = builder.Configuration.GetSection(PingSettings.SectionName).Get<PingSettings>();
if (pingSettings == null) throw new InvalidOperationException($"{nameof(PingSettings)} is not configured. Please check your appsettings.json or environment variables.");

var awsHostSettings = builder.Configuration.GetSection(AwsHostSettings.SectionName).Get<AwsHostSettings>();
if (awsHostSettings == null) throw new InvalidOperationException($"{nameof(AwsHostSettings)} is not configured. Please check your appsettings.json or environment variables.");
new AwsHostSettingsValidator().ValidateAndThrow(awsHostSettings);

var awsTopicSettings = builder.Configuration.GetSection(AwsTopicSettings.SectionName).Get<AwsTopicSettings>();
if (awsTopicSettings == null) throw new InvalidOperationException($"{nameof(AwsTopicSettings)} is not configured. Please check your appsettings.json or environment variables.");
new AwsTopicSettingsValidator().ValidateAndThrow(awsTopicSettings);

var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
if (redisSettings == null) throw new InvalidOperationException($"{nameof(RedisSettings)} is not configured. Please check your appsettings.json or environment variables.");
new RedisSettingsValidator().ValidateAndThrow(redisSettings);

var seqConfigurationSection = builder.Configuration.GetSection(SeqSettings.SectionName);
var seqSettings = seqConfigurationSection.Get<SeqSettings>();
if (seqSettings == null) throw new InvalidOperationException($"{nameof(SeqSettings)} is not configured. Please check your appsettings.json or environment variables.");

var jaegerSettings = builder.Configuration.GetSection(JaegerSettings.SectionName).Get<JaegerSettings>();
if (jaegerSettings == null) throw new InvalidOperationException($"{nameof(JaegerSettings)} is not configured. Please check your appsettings.json or environment variables.");

var betterStackSettings = builder.Configuration.GetSection(BetterStackSettings.SectionName).Get<BetterStackSettings>();
if (betterStackSettings == null) throw new InvalidOperationException($"{nameof(BetterStackSettings)} is not configured. Please check your appsettings.json or environment variables.");

// Logging
builder.Logging
    .AddOpenTelemetry(options => options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName)))
    .AddSimpleConsole()
    .AddSeq(seqConfigurationSection);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
            // .AddHttpClientInstrumentation() //OpenTelemetry.Instrumentation.Http
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opt =>
            {
                // Jaeger
                // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

                // Seq
                // opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                // if (!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

                //BetterStack
                opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/traces");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
            })
            .AddSource(nameof(EvictCacheItemMessageV1)) // Should be the name of any activities used in code
            .AddSource(nameof(EvictCacheAllMessageV1)) // Should be the name of any activities used in code
        // .AddConsoleExporter()
    )
    .WithMetrics(metrics => metrics
            // .AddHttpClientInstrumentation() //OpenTelemetry.Instrumentation.Http
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opt =>
            {
                // Jaeger
                // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

                // Seq
                // opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                // if(!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

                //BetterStack
                opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/metrics");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
            })
        // .AddConsoleExporter()
    )
    .WithLogging(logging =>
    {
        logging.AddOtlpExporter(opt =>
        {
            // Jaeger
            // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

            // Seq
            // opt.Endpoint = new Uri(seqSettings.OpenTelemetryLogIngestUrl);
            // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            // if (!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

            //BetterStack
            opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/logs");
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
        });
    });

// Exception Handling
builder.Services.AddExceptionHandling(options => { options.Environment = builder.Environment; });

builder.Services.AddControllers();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

// Authentication and Authorization
builder.Services
    .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.Authority = identityProviderSettings.Authority;
        options.Audience = identityProviderSettings.Audience;
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });
// builder.Services.AddAuthorizationBuilder().AddPolicy("default", policy => { policy.AddAuthenticationSchemes() });


// Health Checks
// The tags correspond to the health check groups, which can be used to filter health checks in the UI or when querying the health status. (EndpointRouteBuilderExtensions.cs)
builder.Services.AddHealthCheckServices(options =>
{
    options.HealthCheckSettings = healthCheckSettings;
    options.PingSettings = pingSettings;
    options.RedisSettings = redisSettings;
});

// Open API / Swagger
builder.Services.AddOpenApiServices();

// This API's Services
builder.Services.AddServices(options =>
{
    options.ClientSettings = clientsSettings;
    options.BetterStackSettings = betterStackSettings;
});

// Caching
builder.Services.AddCachingServices(options =>
{
    options.CacheType = contentDeliverySettings.CachingSettings.CacheType;
    options.CacheDurationTimeSpan = contentDeliverySettings.CachingSettings.CacheDurationTimeSpan;
    options.RedisConnectionString = redisSettings.ConnectionString;
});

// Workers
builder.Services.AddHostedService<LocalFilesCachingWorker>();

if (contentDeliverySettings.CachingSettings.EvictCachingEnabled)
{
    // Add Topica MessagingPlatform Components
    await builder.Services.AddMessagingServices(options =>
    {
        options.AwsHostSettings = awsHostSettings;
        options.AwsTopicSettings = awsTopicSettings;
    }, cancellationToken);

    builder.Services.AddHostedService<WebUpdateConsumerWorker>();
    builder.Services.AddHostedService<WebUpdateProducerWorker>();
}

// Host Options
builder.Services.Configure<HostOptions>(options => { options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore; });


//////////////////////////
// Configure App

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/memquranapi.json", "memquranapi"); });

// app.UseMiddleware<ExceptionMiddleware>(); // Old way: Custom middleware for handling exceptions
app.UseExceptionHandler(); // New Way: Use built-in exception handler middleware

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Custom Middleware for Health Checks
app.MapCustomHealthCheck(Enum.GetValues<HealthCheckTag>().Select(x => x.ToString()).ToArray());
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health";
    options.ApiPath = "/healthapi";
    options.WebhookPath = "/healthwebhook";
    options.AddCustomStylesheet("HealthChecks/css/healthchecksui.css");
});

// Application Lifetime - cancellation tokens for application lifetime - graceful shutdown
var appStartedCancellationToken = app.Lifetime.ApplicationStarted;
appStartedCancellationToken.Register(() => { });

var appStoppingCancellationToken = app.Lifetime.ApplicationStopping;
appStoppingCancellationToken.Register(() => { cts.Cancel(); });

var appStoppedCancellationToken = app.Lifetime.ApplicationStopped;
appStoppedCancellationToken.Register(() => { });

app.Run();

// used for integration tests, need to reference the Program class in tests for WebApplicationFactory
// Can not reference the Program because it is top-level in the file, so we need to declare it as partial
public partial class Program;