namespace NodeTrees.BusinessLogic.Repository
{
    using NodeTrees.BusinessLogic.Models;

    public interface IJournalEventRepository : IGenericRepository<JournalEvent>
    {
        Task<List<JournalEvent>> GetRangeAsync(int skip, int take, DateTime? from, DateTime? to, string? search, CancellationToken ct);
        Task<JournalEvent?> GetByIdAsync(long id, CancellationToken ct);
    }
}
