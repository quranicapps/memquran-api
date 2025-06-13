using MemQuran.Api.Contracts;

namespace MemQuran.Api.Clients.Local;

public class LocalFileClient : ICdnClient
{
    public async Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine("..", "..", "..", "..", $"QuranStatic/static/{filePath}");
        
        if (!File.Exists(fullFilePath))
        {
            return null;
        }
        
        using var streamReader = File.OpenText(fullFilePath);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public async Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine("..", "..", "..", "..", $"QuranStatic/static/{filePath}");
        
        if (!File.Exists(fullFilePath))
        {
            return null;
        }
        
        return await File.ReadAllBytesAsync(fullFilePath, cancellationToken);
    }
}