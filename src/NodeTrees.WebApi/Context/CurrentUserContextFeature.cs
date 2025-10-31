namespace NodeTrees.WebApi.Context
{
    using NodeTrees.BusinessLogic.Context;

    public interface ICurrentUserContextFeature
    {
        ICurrentUserContext Context { get; }
    }

    public sealed class CurrentUserContextFeature(ICurrentUserContext context) : ICurrentUserContextFeature
    {
        public ICurrentUserContext Context { get; } = context;
    }
}
