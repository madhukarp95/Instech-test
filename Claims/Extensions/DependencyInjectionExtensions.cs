using System.Text.Json.Serialization;

namespace Claims.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Configures the dependencies for the Claims module.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>

    public static async Task ConfigureDependenciesAsync(this WebApplicationBuilder builder)
    {
        await builder.Services.ConfigureDatabaseServices(builder.Configuration);

        builder.Services.ConfigureDependentServices();

        builder.Services.AddProblemDetails();

        builder.Services.ConfigureVersioningServices();

        builder.Services.ConfigureSwaggerServices();

        // Add services to the container.
        builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}
