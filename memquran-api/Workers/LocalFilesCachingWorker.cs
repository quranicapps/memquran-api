using System.Diagnostics;
using QuranApi.Contracts;
using QuranApi.Models;
using QuranApi.Settings;

namespace QuranApi.Workers;

public class LocalFilesCachingWorker : BackgroundService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly ContentDeliverySettings _contentDeliverySettings;
    private readonly ILogger<LocalFilesCachingWorker> _logger;

    public LocalFilesCachingWorker(ICachingProviderFactory cachingProviderFactory, ContentDeliverySettings contentDeliverySettings, ILogger<LocalFilesCachingWorker> logger)
    {
        _cachingProvider = cachingProviderFactory.GetCachingProvider(contentDeliverySettings.CachingSettings.CacheType);
        _contentDeliverySettings = contentDeliverySettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_contentDeliverySettings.Type != ContentDeliveryType.Local)
        {
            _logger.LogInformation("Caching is disabled for non local content");
            return;
        }
        
        if (_contentDeliverySettings.CachingSettings.CacheType == CacheType.None)
        {
            _logger.LogInformation("CacheType.None set in settings, so not going to cache");
            return;
        }
        
        if (!_contentDeliverySettings.CachingSettings.InitialCachingEnabled)
        {
            _logger.LogInformation("Initial Caching is disabled in settings");
            return;
        }

        var sw = Stopwatch.StartNew();
        
        var filePaths = new List<string>
        {
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/surahInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/juzInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/pageInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/rukuInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/maqraInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/surahs"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/juzs"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/pages"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/rukus"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/maqras"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/verses"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/reciterInfos"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/audio"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/duas"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/namesOfAllah"),
            Path.Combine("..", "..", "..", "..", "QuranStatic/static/json/memorise"),
        };

        await Parallel.ForEachAsync(filePaths, stoppingToken, async (path, cancellationToken) => { await CacheFiles(path, "*.json", SearchOption.AllDirectories, cancellationToken); });

        _logger.LogInformation("Finished all Caching in {Time}", sw.Elapsed);
    }

    private async Task CacheFiles(string path,
        string fileExtensionGlob = "*.json",
        SearchOption searchOption = SearchOption.AllDirectories,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var filePaths = Directory.GetFiles(path, fileExtensionGlob, searchOption).ToList();
        
        await Parallel.ForEachAsync(filePaths, cancellationToken, async (filePath, ct) =>
        {
            var surahsBytes = await File.ReadAllBytesAsync(filePath, ct);
            var cacheKey = Path.GetFileName(filePath);
            await _cachingProvider.SetAsync($"{cacheKey}", surahsBytes, ct);
        });

        _logger.LogInformation("Cached: {PathFileName} - {Count} ({Glob}) files in {Time}", Path.GetFileName(path), filePaths.Count, fileExtensionGlob, sw.Elapsed);
    }
}