using MemQuran.Api.Settings.Messaging;
using MemQuran.Core.Models;

namespace MemQuran.Api.Configuration.ApiServices;

public class ApiConfiguration
{
    public string JsDelivrServiceBaseUrl { get; set; } = null!;
    public TimeSpan JsDelivrServiceDefaultTimeout { get; set; }
    
    public CacheType CacheType { get; set; }
    public string RedisConnectionString { get; set; } = null!;

    public int HealthCheckTimeoutSeconds { get; set; }

    public IWebHostEnvironment Environment { get; set; } = null!;

    public AwsHostSettings AwsHostSettings { get; set; } = null!;
    public AwsConsumerSettings AwsConsumerSettings { get; set; } = null!;
}