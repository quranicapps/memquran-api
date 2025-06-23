using MemQuran.Core.Models;

namespace MemQuran.Core.Contracts;

public interface ICdnClient
{
    ContentDeliveryType Name { get; }
    Task<string?> GetFileContentStringAsync(string filePath, CancellationToken cancellationToken = default);
    Task<byte[]> GetFileContentBytesAsync(string filePath, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default);
}