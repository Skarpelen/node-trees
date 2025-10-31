namespace NodeTrees.BusinessLogic.Services
{
    using NodeTrees.BusinessLogic.Repository;

    public readonly record struct CreateRefreshTokenResult(Guid Jti); //, string Refresh);

    public interface ITokenService
    {
        CreateRefreshTokenResult Create();
    }

    public sealed class TokenService() : ITokenService
    {
        public CreateRefreshTokenResult Create()
        {
            var jti = Guid.NewGuid();
            return new CreateRefreshTokenResult(jti);
        }
    }
}
