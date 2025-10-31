namespace NodeTrees.BusinessLogic.Repository
{
    public interface IUnitOfWork
    {
        ITreeRepository Trees { get; }
        ITreeNodeRepository TreeNodes { get; }
        IJournalEventRepository JournalEvents { get; }
        IUserRepository Users { get; }
        Task CompleteAsync(CancellationToken token = default);

        Task BeginTransactionAsync(CancellationToken token = default);
        Task CommitTransactionAsync(CancellationToken token = default);
        Task RollbackTransactionAsync(CancellationToken token = default);
        void ClearTracking();
    }
}
