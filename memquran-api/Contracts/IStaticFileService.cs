namespace QuranApi.Contracts;

public interface IStaticFileService
{
    Task<string> GetFileCommentAsync(string filePath, CancellationToken cancellationToken = default);
}