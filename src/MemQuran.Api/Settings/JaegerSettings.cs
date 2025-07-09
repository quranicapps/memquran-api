namespace MemQuran.Api.Settings;

public class JaegerSettings
{
    public static string SectionName => nameof(JaegerSettings);
    
    public string Endpoint { get; set; } = null!;
}