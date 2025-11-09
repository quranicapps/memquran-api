namespace MemQuran.Api.Settings;

public class BetterStackSettings
{
    public static string SectionName => nameof(BetterStackSettings);
    
    public string IngestBaseUrl { get; set; } = null!;
    public string BearerToken { get; set; } = null!;
    public TimeSpan DefaultTimeout { get; set; }
}