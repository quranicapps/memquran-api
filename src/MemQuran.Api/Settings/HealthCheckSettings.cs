using MemQuran.Api.Models;

namespace MemQuran.Api.Settings;

public class HealthCheckSettings
{
    public static string SectionName => nameof(HealthCheckSettings);

    public HealthCheckModel LocalFiles { get; set; } = null!;
    public HealthCheckModel JsDelivr { get; set; } = null!;
    public HealthCheckModel Redis { get; set; } = null!;
}

public class HealthCheckModel
{
    public string Name { get; set; } = null!;
    public HealthCheckTag Tag { get; set; }
    public bool Enabled { get; set; }
    public TimeSpan TimeOut { get; set; }
}