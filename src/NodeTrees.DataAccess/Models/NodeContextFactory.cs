using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NodeTrees.DataAccess.Models
{
    public class NodeContextFactory : IDesignTimeDbContextFactory<NodeContext>
    {
        public NodeContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("NODETREES_DB_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Environment variable 'NODETREES_DB_CONNECTION_STRING' is not set.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<NodeContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new NodeContext(optionsBuilder.Options);
        }
    }
}
