using MemQuran.Api.Models;

namespace MemQuran.Api.Settings;

public class HealthCheckSettings
{
    public static string SectionName => nameof(HealthCheckSettings);

    public HealthCheckModel Ping { get; init; } = null!;
    public HealthCheckModel LocalFiles { get; init; } = null!;
    public HealthCheckModel JsDelivr { get; init; } = null!;
    public HealthCheckModel Redis { get; init; } = null!;
    public HealthCheckModel BetterStack { get; init; } = null!;
}

public class HealthCheckModel
{
    public string Name { get; set; } = null!;
    public HealthCheckTag Tag { get; set; }
    public bool Enabled { get; set; }
    public TimeSpan TimeOut { get; set; }
}