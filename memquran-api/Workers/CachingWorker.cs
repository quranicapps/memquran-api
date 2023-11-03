using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Settings;

namespace QuranApi.Workers;

public class CachingWorker : BackgroundService
{
    private readonly IDistributedCache _cache;
    private readonly CachingSettings _cachingSettings;
    private readonly ILogger<CachingWorker> _logger;

    public CachingWorker(IDistributedCache cache, CachingSettings cachingSettings, ILogger<CachingWorker> logger)
    {
        _cache = cache;
        _cachingSettings = cachingSettings;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Cache stuff on start
        foreach (var languageCode in new[] { "en" })
        {
            var surahsBytes = await File.ReadAllBytesAsync($"Resources/surahInfos/{languageCode}_surahInfos.json", stoppingToken);
            await _cache.SetAsync($"surahInfos-{languageCode}", surahsBytes, token: stoppingToken);
            _logger.LogInformation("{Name} - Cached: surahInfos-{LanguageCode}", nameof(CachingWorker), languageCode);
        }
        
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     var encodedCachedTimeUtc = await _cache.GetAsync("cachedTimeUTC", stoppingToken);
        //     if(encodedCachedTimeUtc == null) continue;
        //     _logger.LogInformation("cachedTimeUTC: {time}", Encoding.UTF8.GetString(encodedCachedTimeUtc));
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}