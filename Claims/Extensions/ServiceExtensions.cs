using Claims.BackgroundJob;
using Claims.Services.Audit;
using Claims.Services.Channels;
using Claims.Services.Claims;
using Claims.Services.Coverage;
using Claims.Services.PremiumCalculator;

namespace Claims.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers the dependent services for the Claims module.
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureDependentServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IAuditor, Auditor>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICoverService, CoverService>();

        services.AddSingleton<IChannelQueue, ChannelQueue>();
        services.AddHostedService<AuditMessageProcessor>();

        services.AddScoped<YachtCalculator>();
        services.AddScoped<TankerCalculator>();
        services.AddScoped<PassengerShipCalculator>();
        services.AddScoped<IPremiumCalculatorFactory, PremiumCalculatorFactory>();
    }
}
