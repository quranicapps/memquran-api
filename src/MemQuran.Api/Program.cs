using MemQuran.Api.Caching;
using MemQuran.Api.Clients.JsDelivr;
using MemQuran.Api.Clients.Local;
using MemQuran.Api.Contracts;
using MemQuran.Api.Factories;
using MemQuran.Api.Middleware;
using MemQuran.Api.Models;
using MemQuran.Api.Services;
using MemQuran.Api.Settings;
using MemQuran.Api.Workers;

var builder = WebApplication.CreateBuilder();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

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

// var logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Program");

switch (contentDeliverySettings.Type)
{
    // If local, just use the local service client else Add other HTTP clients
    case ContentDeliveryType.Unknown:
    case ContentDeliveryType.Local:
        builder.Services.AddSingleton<ICdnClient, LocalFileClient>();
        logger.LogInformation("Local Files used for Content Delivery");
        break;
    case ContentDeliveryType.JsDelivr:
        builder.Services.AddHttpClient<ICdnClient, JsDelivrClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(clientsSettings.JsDelivrService.BaseUrl);
                httpClient.Timeout = clientsSettings.JsDelivrService.DefaultTimeout;
            })
            .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
        logger.LogInformation("JsDelivrClient used for Content Delivery");
        break;
    case ContentDeliveryType.JsDelivrFallback:
        throw new Exception("Content Delivery Type is not set or is not supported. Please check configuration.");
    default:
        throw new Exception("No Content Delivery Type is set. Please check configuration.");
}

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

app.UseMiddleware<ExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

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