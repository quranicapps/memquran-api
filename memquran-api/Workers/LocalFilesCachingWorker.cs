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
            Path.Combine("..", "..", "static/json/surahInfos"),
            Path.Combine("..", "..", "static/json/surahs"),
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

        _logger.LogInformation("Cached {Count} {Glob} files from {Path} in {Time}", filePaths.Count, fileExtensionGlob, path, sw.Elapsed);
    }
}