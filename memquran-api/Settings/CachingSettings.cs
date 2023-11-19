using QuranApi.Models;

namespace QuranApi.Settings;

public class CachingSettings
{
    public CacheType CacheType { get; set; }
    public bool InitialCachingEnabled { get; set; }
    public bool SlidingExpiration { get; set; }
    public TimeSpan CacheDurationTimeSpan { get; set; }
}