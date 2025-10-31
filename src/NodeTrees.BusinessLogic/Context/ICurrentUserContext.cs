namespace NodeTrees.BusinessLogic.Context
{
    public interface ICurrentUserContext
    {
        string? SecurityStamp { get; }
        string? IpAddress { get; }
        string? UserAgent { get; }
        bool IsAnonymous { get; }
        CancellationToken CancellationToken { get; }
    }
}
