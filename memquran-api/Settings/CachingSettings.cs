using QuranApi.Models;

namespace QuranApi.Settings;

public class CachingSettings
{
    public static string SectionName => "CachingSettings";
    
    public bool Enabled { get; set; }
    public bool SlidingExpiration { get; set; }
    public CacheType CacheType { get; set; }
    public TimeSpan CacheDurationTimeSpan { get; set; }
}