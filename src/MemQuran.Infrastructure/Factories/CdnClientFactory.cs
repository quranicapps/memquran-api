using MemQuran.Core.Contracts;
using MemQuran.Core.Models;

namespace MemQuran.Infrastructure.Factories;

public class CdnClientFactory(IEnumerable<ICdnClient> clients) : ICdnClientFactory
{
    public ICdnClient Create(ContentDeliveryType contentDeliveryType)
    {
        return clients.FirstOrDefault(c => c.Name == contentDeliveryType) ?? throw new NotSupportedException($"Content delivery type '{contentDeliveryType}' is not supported. Available types: {string.Join(", ", clients.Select(c => c.Name))}");
    }
}