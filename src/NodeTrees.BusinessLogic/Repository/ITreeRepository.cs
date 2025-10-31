namespace NodeTrees.BusinessLogic.Repository
{
    using NodeTrees.BusinessLogic.Models;

    public interface ITreeRepository : IGenericRepository<Tree>
    {
        Task<Tree?> GetByNameForUserAsync(long userId, string name, CancellationToken ct);
    }
}
