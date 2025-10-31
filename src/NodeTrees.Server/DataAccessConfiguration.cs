using Microsoft.EntityFrameworkCore;
using NLog;

namespace NodeTrees.Server
{
    using NodeTrees.DataAccess.Models;

    internal static class DataAccessConfiguration
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void ConfigureDatabase(this WebApplicationBuilder builder)
        {
            var usedDatabase = builder.Configuration["UsedDatabase"];
            var dbConnectConfig = builder.Configuration.GetRequiredSection("Databases").GetRequiredSection(usedDatabase!);

            if (usedDatabase == "Postgres")
            {
                var connectionString = dbConnectConfig["ConnectionString"];
                builder.Services.AddDbContext<NodeContext>(options =>
                    options
                        .EnableSensitiveDataLogging()
                        .UseNpgsql(connectionString));
            }
            //else if (usedDatabase == "InMemory")
            //{
            //    var databaseName = dbConnectConfig["DatabaseName"]!;
            //    builder.Services.AddDbContext<NodeContext>(options =>
            //        options
            //            .EnableSensitiveDataLogging()
            //            .UseInMemoryDatabase(databaseName));
            //}
            else
            {
                throw new Exception("Unknown database type");
            }
        }

        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<NodeContext>();
            _log.Info("Applying database migrations...");
            await context.Database.MigrateAsync();
            _log.Info("Database migrations applied successfully.");
        }
    }
}
