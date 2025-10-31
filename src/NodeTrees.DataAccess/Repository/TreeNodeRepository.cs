using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public class TreeNodeRepository(NodeContext context)
        : GenericRepository<TreeNode>(context), ITreeNodeRepository
    {
        public async Task<TreeNode?> GetWithTreeAsync(long nodeId, CancellationToken ct)
        {
            var query = context.Nodes
                .AsNoTracking()
                .Include(n => n.Tree)
                .Where(n => !n.IsDeleted && n.Id == nodeId);

            var node = await query.FirstOrDefaultAsync(ct);
            return node;
        }

        public async Task<TreeNode?> GetRootByTreeIdAsync(long treeId, CancellationToken ct)
        {
            var query = context.Nodes
                .AsNoTracking()
                .Where(n => !n.IsDeleted && n.TreeId == treeId && n.ParentId == null);

            var node = await query.FirstOrDefaultAsync(ct);
            return node;
        }

        public async Task<List<TreeNode>> GetByTreeIdAsync(long treeId, CancellationToken ct)
        {
            var query = context.Nodes
                .AsNoTracking()
                .Where(n => !n.IsDeleted && n.TreeId == treeId)
                .OrderBy(n => n.Path)
                .ThenBy(n => n.Name);

            var list = await query.ToListAsync(ct);
            return list;
        }

        public async Task<List<TreeNode>> GetDescendantsAsync(long nodeId, CancellationToken ct)
        {
            var node = await context.Nodes
                .AsNoTracking()
                .Where(n => !n.IsDeleted && n.Id == nodeId)
                .Select(n => new { n.TreeId, n.Path })
                .FirstOrDefaultAsync(ct);

            if (node == null || string.IsNullOrWhiteSpace(node.Path))
            {
                return new List<TreeNode>();
            }

            var prefix = node.Path + "/%";

            var query = context.Nodes
                .AsNoTracking()
                .Where(n => !n.IsDeleted &&
                            n.TreeId == node.TreeId &&
                            n.Path != null &&
                            EF.Functions.Like(n.Path, prefix));

            var list = await query.ToListAsync(ct);
            return list;
        }

        public async Task<bool> SiblingNameExistsAsync(long treeId, long parentNodeId, string nodeName, CancellationToken ct)
        {
            var name = nodeName.Trim();

            var exists = await context.Nodes
                .AsNoTracking()
                .AnyAsync(n =>
                    !n.IsDeleted &&
                    n.TreeId == treeId &&
                    n.ParentId == parentNodeId &&
                    n.Name == name, ct);

            return exists;
        }

        public async Task HardDeleteByTreeId(long treeId, CancellationToken ct)
        {
            await context.Nodes
                .Where(n => n.TreeId == treeId)
                .ExecuteDeleteAsync(ct);
        }

        public async Task HardDeleteSubtree(long nodeId, CancellationToken ct)
        {
            var info = await context.Nodes
                .Where(n => n.Id == nodeId)
                .Select(n => new { n.TreeId, n.Path })
                .FirstOrDefaultAsync(ct);

            if (info == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(info.Path))
            {
                await context.Nodes
                    .Where(n => n.Id == nodeId)
                    .ExecuteDeleteAsync(ct);

                return;
            }

            var prefix = info.Path + "/%";

            await context.Nodes
                .Where(n =>
                    n.TreeId == info.TreeId &&
                    (
                        (n.Path != null && EF.Functions.Like(n.Path, prefix))
                        || n.Id == nodeId
                    ))
                .ExecuteDeleteAsync(ct);
        }
    }
}
