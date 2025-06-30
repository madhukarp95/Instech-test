using Claims.Persistance;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Extensions;

public static class DatabaseConfigurationExtensions
{
    /// <summary>
    /// Configures the database services for the Claims module using Test containers for SQL Server and MongoDB.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static async Task ConfigureDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
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
        {
            var client = new MongoClient(mongoContainer.GetConnectionString());
            var database = client.GetDatabase(configuration["MongoDb:DatabaseName"]); // Use a default/test database name
            options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        });
    }
}
