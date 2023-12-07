using Microsoft.Extensions.Logging.Console;
using QuranApi.Clients.Local;
using QuranApi.Contracts;
using QuranApi.Models;
using QuranApi.Settings;
using QuranApi.Workers;

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

// var logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger("Program");

// If local, just use the local service client else Add other HTTP clients
if (contentDeliverySettings.Type == ContentDeliveryType.Local)
{
    builder.Services.AddSingleton<ICdnClient, LocalFileClient>();
    logger.LogInformation("Local Files used for Content Delivery");
}
else
{
    builder.Services.AddHttpClient<ICdnClient, JsDelivrClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(clientsSettings.JsDelivrService.BaseUrl);
            httpClient.Timeout = clientsSettings.JsDelivrService.DefaultTimeout;
        })
        .AddHttpMessageHandler(() => new JsDelivrDelegatingHandler());
    logger.LogInformation("JsDelivrClient used for Content Delivery");
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