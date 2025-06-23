using MemQuran.Core.Models;

namespace MemQuran.Core.Contracts;

public interface ICdnClientFactory
{
    ICdnClient Create(ContentDeliveryType contentDeliveryType);
}