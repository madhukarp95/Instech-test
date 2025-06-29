using Claims.Models;
using System.Threading.Channels;

namespace Claims.Services.Channels;

public class ChannelQueue : IChannelQueue
{
    private readonly Channel<ChannelRequest> _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelQueue"/> class.
    /// </summary>
    public ChannelQueue()
    {
        _channel = Channel.CreateUnbounded<ChannelRequest>(
            new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = false
            });
    }

    // <inheritdoc />
    public ChannelReader<ChannelRequest> Reader => _channel.Reader;

    // <inheritdoc />
    public async ValueTask EnqueueAsync(ChannelRequest item) => await _channel.Writer.WriteAsync(item);
}
