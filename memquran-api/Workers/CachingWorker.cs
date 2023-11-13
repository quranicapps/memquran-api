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
        // Cache Surah Infos files
        foreach (var surahInfoFile in Directory.GetFiles("Resources/surahInfos"))
        {
            var surahsBytes = await File.ReadAllBytesAsync(surahInfoFile, stoppingToken);
            var cacheKey = Path.GetFileNameWithoutExtension(surahInfoFile);
            await _cache.SetAsync($"{cacheKey}", surahsBytes, token: stoppingToken);
        }
        _logger.LogInformation("{Name} - Cached: {{locale}}_surahInfos", nameof(CachingWorker));

        
        // Cache Surah files
        foreach (var surahInfoFile in Directory.GetFiles("Resources/surahs", "*.json", SearchOption.AllDirectories))
        {
            var surahsBytes = await File.ReadAllBytesAsync(surahInfoFile, stoppingToken);
            var cacheKey = Path.GetFileNameWithoutExtension(surahInfoFile);
            await _cache.SetAsync($"{cacheKey}", surahsBytes, token: stoppingToken);
            // _logger.LogInformation("{Name} - Cached: {CacheKey}", nameof(CachingWorker), cacheKey);
        }
        _logger.LogInformation("{Name} - Cached: surahs", nameof(CachingWorker));
    }
}