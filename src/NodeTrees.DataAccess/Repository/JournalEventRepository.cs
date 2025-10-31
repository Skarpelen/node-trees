using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public class JournalEventRepository(NodeContext context)
        : GenericRepository<JournalEvent>(context), IJournalEventRepository
    {
        public async Task<JournalEvent?> GetByIdAsync(long id, CancellationToken ct)
        {
            var ent = await context.JournalEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.EventId == id, ct);

            return ent;
        }

        public async Task<List<JournalEvent>> GetRangeAsync(int skip, int take, DateTime? from, DateTime? to, string? search, CancellationToken ct)
        {
            var query = context.JournalEvents
                .AsNoTracking()
                .Where(e => !e.IsDeleted);

            if (from.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                query = query.Where(e => e.TimestampUtc >= fromUtc);
            }

            if (to.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                query = query.Where(e => e.TimestampUtc <= toUtc);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = $"%{search.Trim()}%";
                query = query.Where(e =>
                    (e.Message != null && EF.Functions.Like(e.Message, term)) ||
                    (e.ExceptionType != null && EF.Functions.Like(e.ExceptionType, term)) ||
                    (e.Path != null && EF.Functions.Like(e.Path, term)));
            }

            query = query
                .OrderByDescending(e => e.TimestampUtc)
                .ThenByDescending(e => e.Id);

            var list = await query.Skip(skip).Take(take).ToListAsync(ct);
            return list;
        }
    }
}
