using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public abstract class GenericRepository<T>(NodeContext context) : IGenericRepository<T>
        where T : BaseModel
    {
        protected readonly NodeContext _context = context;

        public async Task<IEnumerable<T>> GetAll(bool includeDeleted = false, CancellationToken ct = default)
        {
            return includeDeleted
                ? await _context.Set<T>().ToListAsync(ct)
                : await _context.Set<T>().Where(e => !e.IsDeleted).ToListAsync(ct);
        }

        public async Task<IEnumerable<T>> GetAllByIds(IEnumerable<long> ids, bool includeDeleted = false, CancellationToken ct = default)
        {
            var idSet = ids.ToHashSet();
            return await _context.Set<T>()
                .Where(e => (includeDeleted || !e.IsDeleted) && idSet.Contains(e.Id))
                .ToListAsync(ct);
        }

        public async Task<T?> Get(long id, bool includeDeleted = false, CancellationToken ct = default)
        {
            var entity = await _context.Set<T>().FindAsync([id], ct);

            if (entity == null)
            {
                return null;
            }

            if (includeDeleted)
            {
                return entity;
            }

            return entity.IsDeleted == false ? entity : null;
        }

        public async Task Add(T entity, CancellationToken ct = default)
        {
            entity.MarkCreated();
            await _context.Set<T>().AddAsync(entity, ct);
        }

        public Task Update(T entity)
        {
            entity.MarkUpdated();
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task SoftDelete(long id, CancellationToken ct = default)
        {
            var entity = await Get(id, false, ct);

            if (entity == null)
            {
                return;
            }

            entity.Delete();
            await Update(entity);
        }

        public async Task HardDelete(long id, CancellationToken ct = default)
        {
            var entity = await Get(id, true, ct);

            if (entity == null)
            {
                return;
            }

            _context.Set<T>().Remove(entity);
        }

        public Task<bool> Exists(long id, bool includeDeleted = false, CancellationToken ct = default)
        {
            return includeDeleted
                ? _context.Set<T>().AnyAsync(e => e.Id == id, ct)
                : _context.Set<T>().Where(e => !e.IsDeleted).AnyAsync(e => e.Id == id, ct);
        }
    }
}
