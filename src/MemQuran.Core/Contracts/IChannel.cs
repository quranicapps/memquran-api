using System.Threading.Channels;

namespace MemQuran.Core.Contracts;

public interface IMessagingChannel
{
    Channel<T> CreateBounded<T>(int maxMessages = 250, bool singleReader = true, bool singleWriter = true);
    Channel<T> CreateUnbounded<T>(bool allowSynchronousContinuations = true, bool singleReader = true, bool singleWriter = true);
    Channel<T> CreateUnboundedPrioritized<T>(bool allowSynchronousContinuations = true, bool singleReader = true, bool singleWriter = true, IComparer<T>? comparer = null);
}