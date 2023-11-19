namespace QuranApi.Clients.Local;

public class LocalFileClient : ICdnClient
{
    public async Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine("..", "..", $"static/{filePath}");
        
        if (!File.Exists(fullFilePath))
        {
            return null;
        }
        
        using var streamReader = File.OpenText(fullFilePath);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }
}