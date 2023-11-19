using System.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using QuranApi.Settings;

namespace QuranApi.Workers;

public class CachingWorker(IDistributedCache cache, CachingSettings cachingSettings, ILogger<CachingWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!cachingSettings.Enabled)
        {
            logger.LogInformation("Caching is disabled in settings");
            return;
        }

        var sw = Stopwatch.StartNew();

        
        var filePaths = new List<string>
        {
            Path.Combine("..", "..", "static/json/surahInfos"),
            Path.Combine("..", "..", "static/json/surahs"),
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
            var surahsBytes = await File.ReadAllBytesAsync(filePath, ct);
            var cacheKey = Path.GetFileNameWithoutExtension(filePath);
            await cache.SetAsync($"{cacheKey}", surahsBytes, token: ct);
        });

        logger.LogInformation("Cached {Count} {Glob} files from {Path} in {Time}", filePaths.Count, fileExtensionGlob, path, sw.Elapsed);
    }
}