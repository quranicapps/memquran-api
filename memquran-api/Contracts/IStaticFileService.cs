namespace QuranApi.Contracts;

public interface IStaticFileService
{
    Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default);
}