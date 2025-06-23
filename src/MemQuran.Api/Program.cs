using System.Diagnostics;
using MemQuran.Api.Clients.JsDelivr;
using MemQuran.Api.Clients.Local;
using MemQuran.Api.Extensions;
using MemQuran.Api.HealthChecks;
using MemQuran.Api.Middleware;
using MemQuran.Api.Services;
using MemQuran.Api.Settings;
using MemQuran.Api.Workers;
using MemQuran.Core.Contracts;
using MemQuran.Core.Models;
using MemQuran.Infrastructure.Caching;
using MemQuran.Infrastructure.Factories;
using MemQuran.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(opts => // built-in problem details support
    opts.CustomizeProblemDetails = ctx =>
    {
        if (!ctx.ProblemDetails.Extensions.ContainsKey("traceId"))
        {
            var traceId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
            ctx.ProblemDetails.Extensions.Add(new KeyValuePair<string, object?>("traceId", traceId));
        }

        if (!ctx.ProblemDetails.Extensions.ContainsKey("instance"))
        {
            ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
        }

        var exception = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (ctx.ProblemDetails.Status != 500) return;

        ctx.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";

        if (builder.Environment.IsProduction()) return;

        if (!ctx.ProblemDetails.Extensions.ContainsKey("errorMessage"))
        {
            ctx.ProblemDetails.Extensions.Add("errorMessage", exception?.Message);
        }

        if (!ctx.ProblemDetails.Extensions.ContainsKey("stackTrace"))
        {
            ctx.ProblemDetails.Extensions.Add("stackTrace", exception?.StackTrace);
        }
    }
);
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

// Health Checks and Health Checks UI (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
builder.Services.AddHealthChecks()
    .AddCheck("API Running", () => Healthy(), tags: ["health"])
    .AddCheck<JsDelivrHealthCheck>("Call JsDelivr", timeout: TimeSpan.FromSeconds(5), tags: new List<string> { "services", "cdn" })
    .AddCheck<LocalCdnHealthCheck>("Call Local CDN", timeout: TimeSpan.FromSeconds(5), tags: new List<string> { "services", "cdn" })
    .AddUrlGroup(new Uri("http://httpbin.org/status/200"), name: "http connection check", tags: new List<string> { "services", "http", "internet" })
    .AddUrlGroup(new Uri("https://httpbin.org/status/200"), name: "https connection check", tags: new List<string> { "services", "https", "internet" });
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

// Configuration
var contentDeliverySettings = builder.Configuration.GetSection(ContentDeliverySettings.SectionName).Get<ContentDeliverySettings>();
if (contentDeliverySettings == null) throw new Exception("Could not bind the Content Delivery Settings, please check configuration");
builder.Services.AddSingleton(contentDeliverySettings);

var clientsSettings = builder.Configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
if (clientsSettings == null) throw new Exception("Could not bind the Clients Settings, please check configuration");
builder.Services.AddSingleton(clientsSettings);

// Caching
builder.Services.AddDistributedMemoryCache(options => { options.SizeLimit = long.MaxValue; });
builder.Services.AddSingleton<ICachingProviderFactory, CachingProviderFactory>();
builder.Services.AddSingleton<ICachingProvider, NullCachingProvider>();
builder.Services.AddSingleton<ICachingProvider, MemoryCachingProvider>();

// Workers
builder.Services.AddHostedService<LocalFilesCachingWorker>();

// Services
builder.Services.AddSingleton<IHashingService, HashingService>();
builder.Services.AddSingleton<IStaticFileService, StaticFileService>();
builder.Services.AddScoped<JsDelivrDelegatingHandler>();
builder.Services.AddSingleton<ICdnClient, LocalFileClient>();
builder.Services.AddHttpClient<ICdnClient, JsDelivrClient>(httpClient =>
    {
        httpClient.BaseAddress = new Uri(clientsSettings.JsDelivrService.BaseUrl);
        httpClient.Timeout = clientsSettings.JsDelivrService.DefaultTimeout;
    })
    .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
builder.Services.AddSingleton<ICdnClientFactory, CdnClientFactory>();

var logger = LoggerFactory.Create(loggingBuilder => loggingBuilder
        .AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.IncludeScopes = true;
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ffff ";
        })
        .AddFilter(level => level >= LogLevel.Information))
    .CreateLogger("Program");

logger.LogInformation("Starting Env: {Env} on port {Port}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "?", Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "?");

// Host Options
builder.Services.Configure<HostOptions>(options => { options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore; });

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<ExceptionMiddleware>(); // Old way: Custom middleware for handling exceptions
app.UseExceptionHandler(); // New Way: Use built-in exception handler middleware

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Custom Middleware for Health Checks
app.MapCustomHealthCheck();
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health";
    options.ApiPath = "/healthapi";
    options.WebhookPath = "/healthwebhook";
    options.AddCustomStylesheet("HealthChecks/css/healthchecksui.css");
});

// Startup when application starts
// app.Lifetime.ApplicationStarted.Register(() =>
// {
//     var currentTimeUtc = DateTime.UtcNow.ToString();
//     var encodedCurrentTimeUtc = System.Text.Encoding.UTF8.GetBytes(currentTimeUtc);
//     var options = new DistributedCacheEntryOptions()
//         .SetSlidingExpiration(TimeSpan.FromSeconds(20));
//     app.Services.GetService<IDistributedCache>()
//         .Set("cachedTimeUTC", encodedCurrentTimeUtc, options);
// });

app.Run();

// used for integration tests, need to reference the Program class in tests for WebApplicationFactory
// Can not reference the Program because it is top-level in the file, so we need to declare it as partial
public partial class Program;