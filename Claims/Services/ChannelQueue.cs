using Claims.Models;
using Claims.Services.Interfaces;
using System.Threading.Channels;

namespace Claims.Services;

public class ChannelQueue : IChannel
{
    private readonly Channel<ChannelRequest> _channel;
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
