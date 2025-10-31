using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Models
{
    using NodeTrees.BusinessLogic.Models;

    public class NodeContext : DbContext
    {
        public NodeContext(DbContextOptions<NodeContext> options) : base(options)
        {
        }

        public DbSet<TreeNode> Nodes { get; set; } = null!;
        public DbSet<Tree> Trees { get; set; } = null!;
        public DbSet<JournalEvent> JournalEvents { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            foreach (var e in ChangeTracker.Entries<BaseModel>())
            {
                if (e.State == EntityState.Added)
                {
                    if (e.Entity.CreatedAt == default)
                    {
                        e.Entity.MarkCreated();
                    }

                    e.Entity.MarkUpdated();
                }

                if (e.State == EntityState.Modified)
                {
                    e.Entity.MarkUpdated();
                }
            }

            return await base.SaveChangesAsync(ct);
        }
    }
}
