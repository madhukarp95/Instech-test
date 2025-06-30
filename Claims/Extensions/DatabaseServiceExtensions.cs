using Claims.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Extensions
{
    public static class DatabaseServiceExtensions
    {
        /// <summary>
        /// Asynchronously configures the database services for the application.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task ConfigureDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Use null-coalescing operator to handle potential null values
            string mongoDbName = configuration["MongoDb:DatabaseName"] ?? throw new ArgumentException("MongoDB Database name is not configured properly.");

            // Start Test containers for SQL Server and MongoDB
            var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? new MsSqlBuilder()
                        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                    : new()
                ).Build();

            var mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .Build();

            await sqlContainer.StartAsync();
            await mongoContainer.StartAsync();

            services.AddDbContext<AuditContext>(options =>
                            options.UseSqlServer(sqlContainer.GetConnectionString()));

            services.AddDbContext<ClaimsContext>(options =>
                options.UseMongoDB(mongoContainer.GetConnectionString(), mongoDbName));
        }
    }
}
