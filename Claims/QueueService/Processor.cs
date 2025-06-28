using Claims.Services.Interfaces;

namespace Claims.QueueService;

/// <summary>
/// Processor is a background service that processes requests from a channel.
/// https://stackoverflow.com/questions/76809859/when-should-system-threading-channels-be-preferred-to-concurrentqueue
/// https://www.youtube.com/watch?v=lHC38t1w9Nc
/// </summary>
public class Processor : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Processor> _logger;
    public Processor(IChannel channel, IServiceScopeFactory serviceScopeFactory, ILogger<Processor> logger)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessMessageAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var request = await _channel.Reader.ReadAsync(stoppingToken);

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IAuditer scopedService = scope.ServiceProvider.GetRequiredService<IAuditer>();

                if (request.type == "Claims")
                {
                    await scopedService.AuditClaim(request.Id, request.HttpRequestType);
                }
                else if (request.type == "Covers")
                {
                    await scopedService.AuditCover(request.Id, request.HttpRequestType);
                }

                _logger.LogInformation("Processing {RequestType} request with ID - {request.Id} for {CoverType} type.",
                    request.HttpRequestType, request.Id, request.type);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in background service");
            throw;
        }
    }
}
