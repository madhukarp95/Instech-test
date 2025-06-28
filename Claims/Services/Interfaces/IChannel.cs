using Claims.Models;
using System.Threading.Channels;

namespace Claims.Services.Interfaces;

public interface IChannel
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
