using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Settings;
using QuranApi.Workers;

var builder = WebApplication.CreateBuilder();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin());
});

// Configuration
var cachingSettings = builder.Configuration.GetSection(CachingSettings.SectionName).Get<CachingSettings>();
if (cachingSettings == null) throw new Exception("Could not bind the caching settings, please check configuration");

builder.Services.AddSingleton(cachingSettings);
builder.Services.AddDistributedMemoryCache();

// Workers
builder.Services.AddHostedService<CachingWorker>();

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

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