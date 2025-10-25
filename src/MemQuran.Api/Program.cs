using Azure.Identity;
using FluentValidation;
using MemQuran.Api.Messaging;
using MemQuran.Api.Models;
using MemQuran.Api.Settings;
using MemQuran.Api.Settings.Messaging;
using MemQuran.Api.Validators;
using MemQuran.Api.Workers;
using OpenTelemetry.Exporter;
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
var contentDeliverySettings = builder.Configuration.GetSection(ContentDeliverySettings.SectionName).Get<ContentDeliverySettings>();
if (contentDeliverySettings == null) throw new Exception("Could not bind the Content Delivery Settings, please check configuration");
builder.Services.AddSingleton(contentDeliverySettings);

var clientsSettings = builder.Configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
if (clientsSettings == null) throw new Exception("Could not bind the Clients Settings, please check configuration");
builder.Services.AddSingleton(clientsSettings);

var messagingSettings = builder.Configuration.GetSection(MessagingSettings.SectionName).Get<MessagingSettings>();
if (messagingSettings == null) throw new Exception("Could not bind the Messaging Settings, please check configuration");
builder.Services.AddSingleton(messagingSettings);

var awsHostSettings = builder.Configuration.GetSection(AwsHostSettings.SectionName).Get<AwsHostSettings>();
if (awsHostSettings == null) throw new InvalidOperationException($"{nameof(AwsHostSettings)} is not configured. Please check your appsettings.json or environment variables.");
new AwsHostSettingsValidator().ValidateAndThrow(awsHostSettings);

var awsTopicSettings = builder.Configuration.GetSection(AwsTopicSettings.SectionName).Get<AwsTopicSettings>();
if (awsTopicSettings == null) throw new InvalidOperationException($"{nameof(AwsTopicSettings)} is not configured. Please check your appsettings.json or environment variables.");
new AwsTopicSettingsValidator().ValidateAndThrow(awsTopicSettings);

var seqConfigurationSection = builder.Configuration.GetSection(SeqSettings.SectionName);
var seqSettings = seqConfigurationSection.Get<SeqSettings>();
if (seqSettings == null) throw new InvalidOperationException($"{nameof(SeqSettings)} is not configured. Please check your appsettings.json or environment variables.");

var jaegerSettings = builder.Configuration.GetSection(JaegerSettings.SectionName).Get<JaegerSettings>();
if (jaegerSettings == null) throw new InvalidOperationException($"{nameof(JaegerSettings)} is not configured. Please check your appsettings.json or environment variables.");

// Logging
builder.Logging
    .AddOpenTelemetry(options => options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName)))
    .AddSimpleConsole()
    .AddSeq(seqConfigurationSection);

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
                opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                if (!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";
            })
            .AddSource(nameof(EvictCacheItemMessageV1)) // Should be the name of any activities used in code
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
                opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                if(!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";
            })
        // .AddConsoleExporter()
    );

// Exception Handling
builder.Services.AddExceptionHandling(options => { options.Environment = builder.Environment; });

builder.Services.AddControllers();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

// Health Checks
builder.Services.AddHealthCheckServices(config =>
{
    config.ContentDeliverySettings = contentDeliverySettings;
    config.HealthCheckTimeoutSeconds = 5;
    config.RedisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
});

// Open API / Swagger
builder.Services.AddOpenApiServices();

// This API's Services
builder.Services.AddServices(options =>
{
    options.JsDelivrServiceBaseUrl = clientsSettings.JsDelivrService.BaseUrl;
    options.JsDelivrServiceDefaultTimeout = clientsSettings.JsDelivrService.DefaultTimeout;
});

// Caching
builder.Services.AddCachingServices(options =>
{
    options.CacheType = contentDeliverySettings.CachingSettings.CacheType;
    options.RedisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
});

// Workers
builder.Services.AddHostedService<LocalFilesCachingWorker>();

if (messagingSettings.AwsEnabled)
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
app.MapCustomHealthCheck(Enum.GetValues<HealthCheckTags>().Select(x => x.ToString()).ToArray());
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