using MemQuran.Api.Extensions;
using MemQuran.Api.Settings;
using MemQuran.Api.Workers;

var builder = WebApplication.CreateBuilder(args);


////////////////////////////
// Configure Services

builder.Logging.AddSimpleConsole().AddSeq(builder.Configuration.GetSection(SeqSettings.SectionName));

// Exception Handling
builder.Services.AddExceptionHandling(options => { options.Environment = builder.Environment; });

builder.Services.AddControllers();
builder.Services.AddCors(options => { options.AddPolicy("AllowOrigin", policy => policy.AllowAnyOrigin()); });

// Health Checks
builder.Services.AddHealthCheckServices(options => options.HealthCheckTimeoutSeconds = 5);

// Open API / Swagger
builder.Services.AddOpenApiServices(_ => { });

// Configuration
var contentDeliverySettings = builder.Configuration.GetSection(ContentDeliverySettings.SectionName).Get<ContentDeliverySettings>();
if (contentDeliverySettings == null) throw new Exception("Could not bind the Content Delivery Settings, please check configuration");
builder.Services.AddSingleton(contentDeliverySettings);

var clientsSettings = builder.Configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>();
if (clientsSettings == null) throw new Exception("Could not bind the Clients Settings, please check configuration");
builder.Services.AddSingleton(clientsSettings);

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
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/memquranapi.json", "memquranapi"); });
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
//
// });

app.Run();

// used for integration tests, need to reference the Program class in tests for WebApplicationFactory
// Can not reference the Program because it is top-level in the file, so we need to declare it as partial
public partial class Program;