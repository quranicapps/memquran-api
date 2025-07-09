namespace MemQuran.Api.Settings;

public class SeqSettings
{
    public static string SectionName => nameof(SeqSettings);

    public string ServerUrl { get; set; } = null!;
    public string OpenTelemetryTraceIngestUrl { get; set; } = null!;
    public string? ApiKey { get; set; }
    public string? MinimumLevel { get; set; }
    public Dictionary<string, string>? LevelOverride { get; set; }
}

