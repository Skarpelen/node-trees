namespace NodeTrees.BusinessLogic.Repository
{
    using NodeTrees.BusinessLogic.Models;

    public interface ITreeNodeRepository : IGenericRepository<TreeNode>
    {
        Task<TreeNode?> GetWithTreeAsync(long nodeId, CancellationToken ct);
        Task<TreeNode?> GetRootByTreeIdAsync(long treeId, CancellationToken ct);
        Task<List<TreeNode>> GetByTreeIdAsync(long treeId, CancellationToken ct);
        Task<List<TreeNode>> GetDescendantsAsync(long nodeId, CancellationToken ct);
        Task<bool> SiblingNameExistsAsync(long treeId, long parentNodeId, string nodeName, CancellationToken ct);
        Task HardDeleteByTreeId(long treeId, CancellationToken ct);
        Task HardDeleteSubtree(long nodeId, CancellationToken ct);
    }
}
