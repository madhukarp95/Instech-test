namespace Claims.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// Configures the Swagger services for the application.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        /// <summary>
        /// Configures the Swagger middleware for the application.
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureSwagger(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
