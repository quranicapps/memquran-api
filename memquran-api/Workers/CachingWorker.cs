using System.Diagnostics;
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
        if (!_cachingSettings.Enabled)
        {
            _logger.LogInformation("Caching is disabled in settings");
            return;
        }

        var sw = Stopwatch.StartNew();

        var filePaths = new List<string>
        {
            "Resources/surahInfos", "Resources/surahs"
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

        var files = Directory.GetFiles(path, fileExtensionGlob, searchOption).ToList();
        foreach (var surahInfoFile in files)
        {
            var surahsBytes = await File.ReadAllBytesAsync(surahInfoFile, cancellationToken);
            var cacheKey = Path.GetFileNameWithoutExtension(surahInfoFile);
            await _cache.SetAsync($"{cacheKey}", surahsBytes, token: cancellationToken);
        }

        _logger.LogInformation("Cached {Count} {Glob} files from {Path} in {Time}", files.Count, fileExtensionGlob, path, sw.Elapsed);
    }
}