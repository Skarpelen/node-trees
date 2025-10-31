using Microsoft.EntityFrameworkCore.Storage;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NodeContext _context;
        private IDbContextTransaction? _tx;

        public ITreeRepository Trees { get; }

        public ITreeNodeRepository TreeNodes { get; }

        public IJournalEventRepository JournalEvents { get; }

        public IUserRepository Users { get; }

        public UnitOfWork(NodeContext context)
        {
            _context = context;
            Trees = new TreeRepository(_context);
            TreeNodes = new TreeNodeRepository(_context);
            JournalEvents = new JournalEventRepository(_context);
            Users = new UserRepository(_context);
        }

        public async Task CompleteAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
        }

        public async Task BeginTransactionAsync(CancellationToken token = default)
        {
            if (_tx == null)
            {
                _tx = await _context.Database.BeginTransactionAsync(token);
            }
        }

        public async Task CommitTransactionAsync(CancellationToken token = default)
        {
            if (_tx != null)
            {
                await _tx.CommitAsync(token);
                await _tx.DisposeAsync();
                _tx = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_tx != null)
            {
                await _tx.RollbackAsync(token);
                await _tx.DisposeAsync();
                _tx = null;
            }
        }

        public void ClearTracking()
        {
            _context.ChangeTracker.Clear();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
