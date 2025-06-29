using Claims.Services.Audit;
using Claims.Services.Channels;

namespace Claims.BackgroundJob;

/// <summary>
/// Processor is a background service that processes requests from a channel.
/// https://stackoverflow.com/questions/76809859/when-should-system-threading-channels-be-preferred-to-concurrentqueue
/// https://www.youtube.com/watch?v=lHC38t1w9Nc
/// </summary>
public class AuditMessageProcessor : BackgroundService
{
    private readonly IChannelQueue _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AuditMessageProcessor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditMessageProcessor"/> class.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AuditMessageProcessor(IChannelQueue channel, IServiceScopeFactory serviceScopeFactory, ILogger<AuditMessageProcessor> logger)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessMessageAsync(stoppingToken);
    }

    /// <summary>
    /// Execute the background service to process messages from the channel.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    private async Task ProcessMessageAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var request = await _channel.Reader.ReadAsync(stoppingToken);

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAuditor scopedService = scope.ServiceProvider.GetRequiredService<IAuditor>();

                if (request.type == "Claims")
                {
                    await scopedService.AuditClaim(request.Id, request.HttpRequestType);
                }
                else if (request.type == "Covers")
                {
                    await scopedService.AuditCover(request.Id, request.HttpRequestType);
                }

                _logger.LogInformation("Processing {RequestType} request with ID - {Id} for {CoverType} type.",
                    request.HttpRequestType, request.Id, request.type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in background service");
        }
    }
}
