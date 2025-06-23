using System.Net;
using MemQuran.Core.Contracts;
using MemQuran.Core.Models;

namespace MemQuran.Api.Clients.Local;

public class LocalFileClient : ICdnClient
{
    public ContentDeliveryType Name => ContentDeliveryType.Local;
    
    public async Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine("..", "..", "..", "..", "..", $"QuranStatic/static/{filePath}");

        if (!File.Exists(fullFilePath))
        {
            return null;
        }

        using var streamReader = File.OpenText(fullFilePath);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public async Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine("..", "..", "..", "..", "..", $"QuranStatic/static/{filePath}");

        if (!File.Exists(fullFilePath))
        {
            return null;
        }

        return await File.ReadAllBytesAsync(fullFilePath, cancellationToken);
    }

    public async Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        return await GetFileContentStringAsync("health.json", cancellationToken) is not null
            ? new HttpResponseMessage(HttpStatusCode.OK)
            : new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Health check file not found.")
            };
    }
}