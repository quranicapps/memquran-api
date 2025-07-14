using System.Diagnostics;
using MemQuran.Api.Settings;
using MemQuran.Core.Contracts;
using MemQuran.Core.Models;

namespace MemQuran.Api.Workers;

public class LocalFilesCachingWorker(ICachingProviderFactory cachingProviderFactory, ContentDeliverySettings contentDeliverySettings, ILogger<LocalFilesCachingWorker> logger) : BackgroundService
{
    private readonly ICachingProvider _cachingProvider = cachingProviderFactory.GetCachingProvider(contentDeliverySettings.CachingSettings.CacheType);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (contentDeliverySettings.ContentDeliveryType != ContentDeliveryType.Local)
        {
            logger.LogInformation("Caching is disabled for non local content");
            return;
        }
        
        if (contentDeliverySettings.CachingSettings.CacheType == CacheType.None)
        {
            logger.LogInformation("CacheType.None set in settings, so not going to cache");
            return;
        }
        
        if (!contentDeliverySettings.CachingSettings.InitialCachingEnabled)
        {
            logger.LogInformation("Initial Caching is disabled in settings");
            return;
        }

        var sw = Stopwatch.StartNew();
        
        var filePaths = new List<string>
        {
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/surahs"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/juzs"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/pages"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/rukus"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/maqras"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/verses"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/reciterInfos"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/audio"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/duas"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/namesOfAllah"),
            Path.Combine("..", "..", "..", "..", "..", "QuranStatic/static/json/memorise"),
        };

        await Parallel.ForEachAsync(filePaths, stoppingToken, async (path, cancellationToken) => { await CacheFiles(path, "*.json", SearchOption.AllDirectories, cancellationToken); });

        logger.LogInformation("Finished all Caching in {Time}", sw.Elapsed);
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
            var item = await File.ReadAllTextAsync(filePath, ct);
            var cacheKey = Path.GetFileName(filePath);
            await _cachingProvider.SetStringAsync($"{cacheKey}", item, ct);
        });

        logger.LogInformation("Cached: {PathFileName} - {Count} ({Glob}) files in {Time}", Path.GetFileName(path), filePaths.Count, fileExtensionGlob, sw.Elapsed);
    }
}