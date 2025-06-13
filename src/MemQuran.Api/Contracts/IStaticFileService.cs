namespace MemQuran.Api.Contracts;

public interface IStaticFileService
{
    Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default);
    Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default);
}