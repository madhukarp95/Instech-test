using Asp.Versioning;

namespace Claims.Extensions;

public static class ApiVersioningExtension
{
    public static void ConfigureVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0); //same as ApiVersion.Default
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
