namespace NodeTrees.BusinessLogic.Repository
{
    using NodeTrees.BusinessLogic.Models;

    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByCode(string code, CancellationToken ct);
        Task<User?> GetBySecurityStamp(string stamp, CancellationToken ct);
    }
}
