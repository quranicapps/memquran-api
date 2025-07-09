using System.Threading.Channels;
using MemQuran.Core.Contracts;

namespace MemQuran.Infrastructure.Messaging;

public class MessagingChannel : IMessagingChannel
{
    public Channel<T> CreateBounded<T>(int maxMessages = 250, bool singleReader = true, bool singleWriter = true)
    {
        return Channel.CreateBounded<T>(new BoundedChannelOptions(maxMessages)
        {
            SingleReader = singleReader,
            SingleWriter = singleWriter
        });
    }

    public Channel<T> CreateUnbounded<T>(bool allowSynchronousContinuations = true, bool singleReader = true, bool singleWriter = true)
    {
        return Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = allowSynchronousContinuations,
            SingleReader = singleReader,
            SingleWriter = singleWriter
        });
    }

    public Channel<T> CreateUnboundedPrioritized<T>(bool allowSynchronousContinuations = true, bool singleReader = true, bool singleWriter = true, IComparer<T>? comparer = null)
    {
        return Channel.CreateUnboundedPrioritized(new UnboundedPrioritizedChannelOptions<T>
        {
            AllowSynchronousContinuations = allowSynchronousContinuations,
            SingleReader = singleReader,
            SingleWriter = singleWriter,
            Comparer = comparer
        });
    }
}