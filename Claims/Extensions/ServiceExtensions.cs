using Claims.QueueService;
using Claims.Services;
using Claims.Services.Interfaces;

namespace Claims.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDependentServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IAuditer, Auditer>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<ICoverService, CoverService>();

            services.AddSingleton<IChannel, ChannelQueue>();
            services.AddHostedService<Processor>();
        }
    }
}
