using NodeTrees.BusinessLogic.Models;

namespace NodeTrees.BusinessLogic.Repository
{
    public interface IGenericRepository<T>
        where T : BaseModel
    {
        Task<IEnumerable<T>> GetAll(bool includeDeleted = false, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllByIds(IEnumerable<long> ids, bool includeDeleted = false, CancellationToken ct = default);
        Task<T?> Get(long id, bool includeDeleted = false, CancellationToken ct = default);
        Task Add(T entity, CancellationToken ct = default);
        Task Update(T entity);
        Task SoftDelete(long id, CancellationToken ct = default);
        Task HardDelete(long id, CancellationToken ct = default);
        Task<bool> Exists(long id, bool includeDeleted = false, CancellationToken ct = default);
    }
}
