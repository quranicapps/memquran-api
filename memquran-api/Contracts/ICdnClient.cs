public interface ICdnClient
{
    Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default);    
}