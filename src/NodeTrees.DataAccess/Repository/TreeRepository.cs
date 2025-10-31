using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public class TreeRepository(NodeContext context)
        : GenericRepository<Tree>(context), ITreeRepository
    {
        public async Task<Tree?> GetByNameForUserAsync(long userId, string name, CancellationToken ct)
        {
            var query = context.Trees.AsNoTracking().Where(t => t.UserId == userId && t.Name == name);

            return await query.FirstOrDefaultAsync(ct);
        }
    }
}
