namespace MemQuran.Api.Settings;

public class PingSettings
{
    public static string SectionName => nameof(PingSettings);
    
    public ExternalEndpointModel[] ExternalEndpoints { get; set; } = null!;
    public TimeSpan Timeout { get; set; }
}

public class ExternalEndpointModel
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
}

