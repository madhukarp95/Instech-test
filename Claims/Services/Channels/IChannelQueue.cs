using Claims.Models.Channel;
using System.Threading.Channels;

namespace Claims.Services.Channels;

public interface IChannelQueue
{
    /// <summary>
    /// Enqueues an item to the channel asynchronously.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ValueTask EnqueueAsync(ChannelRequest item);

    /// <summary>
    /// Reader for the channel to read items.
    /// </summary>
    ChannelReader<ChannelRequest> Reader { get; }
}
