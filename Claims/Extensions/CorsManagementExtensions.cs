namespace Claims.Extensions
{
    public static class CorsManagementExtensions
    {
        private const string defaultCorsPolicy = "AllowAllOrigins";
        public static void AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(defaultCorsPolicy,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        public static void ApplyCorsDefaultPolicy(this IApplicationBuilder app)
        {
            app.UseCors(defaultCorsPolicy);
        }
    }
}
