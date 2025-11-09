namespace MemQuran.Api.Settings;

public class RedisSettings
{
    public static string SectionName => nameof(RedisSettings);
    
    public string ConnectionString { get; set; } = null!;
}