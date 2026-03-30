using Azure.Identity;
using FluentValidation;
using MemQuran.Api.Models;
using MemQuran.Api.Workers;
using MemQuran.Core.Models;
using MemQuran.Core.Settings;
using MemQuran.Infrastructure.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

////////////////////////////
// Aspire ServiceDefaults project

builder.AddServiceDefaults();

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

var redisConnectionString = builder.Configuration.GetConnectionString("RedisCache");
if (string.IsNullOrWhiteSpace(redisConnectionString)) throw new InvalidOperationException("ConnectionStrings:RedisCache is not configured. Please check your appsettings.json or environment variables.");

// Exception Handling
builder.Services.AddExceptionHandling(options => { options.Environment = builder.Environment; });

builder.Services.AddControllers();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

// Authentication and Authorization
builder.Services
    .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        // Authority and audience comes from "Authentication" in appsettings, which should be set to the Identity Provider (e.g., Auth0) details
        // options.Authority = "";
        // options.Audience = "";
        // options.RequireHttpsMetadata = builder.Environment.IsProduction();

        // The default JWT Bearer handler will automatically retrieve the OpenID Connect metadata from the authority's well-known endpoint (/.well-known/openid-configuration) to get the signing keys and other details, so we don't need to manually set the TokenValidationParameters unless we have specific custom validation requirements. The default behavior is usually sufficient for standard scenarios.
        // options.TokenValidationParameters = new TokenValidationParameters
        // {
        //     ValidateIssuer = true,
        //     ValidIssuer = "",
        //     ValidateAudience = true,
        //     ValidAudience = "",
        //     ValidateLifetime = true,
        //     ValidateIssuerSigningKey = true,
        //     ClockSkew = TimeSpan.FromMinutes(5)
        // };
    });
// builder.Services.AddAuthorizationBuilder().AddPolicy("default", policy => { policy.AddAuthenticationSchemes() });


// Health Checks
// The tags correspond to the health check groups, which can be used to filter health checks in the UI or when querying the health status. (EndpointRouteBuilderExtensions.cs)
builder.Services.AddHealthCheckServices(options =>
{
    options.HealthCheckSettings = healthCheckSettings;
    options.PingSettings = pingSettings;
    options.RedisConnectionString = redisConnectionString;
});

// Open API
builder.Services.AddOpenApi();

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
    options.CacheDurationTimeSpan = contentDeliverySettings.CachingSettings.CacheDurationTimeSpan;
    options.RedisConnectionString = redisConnectionString;
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

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