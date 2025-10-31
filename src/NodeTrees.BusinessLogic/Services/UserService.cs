using Microsoft.Extensions.Options;
using NLog;

namespace NodeTrees.BusinessLogic.Services
{
    using NodeTrees.BusinessLogic.Context;
    using NodeTrees.BusinessLogic.Helpers;
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.Shared;

    public interface IUserService
    {
        Task<string> RememberMeAsync(string code);
        Task<User?> GetUserBySecurityStampAsync(string stamp);
        Task<User> GetCurrentUserAsync(bool throwIfAnonymous);
    }

    public sealed class UserService(IUnitOfWork unitOfWork, ITokenService tokenService, ICurrentUserContext context, IOptions<JwtAuthorizationConfiguration> jwtConfig)
        : IUserService
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task<string> RememberMeAsync(string code)
        {
            var user = await unitOfWork.Users.GetByCode(code, context.CancellationToken);

            if (user == null)
            {
                user = new User()
                {
                    Code = code,
                    Role = UserRole.User
                };

                await unitOfWork.Users.Add(user);
                await unitOfWork.CompleteAsync(context.CancellationToken);
            }

            if (string.IsNullOrWhiteSpace(user.SecurityStamp))
            {
                user.SecurityStamp = Guid.NewGuid().ToString();

                await unitOfWork.CompleteAsync(context.CancellationToken);
            }

            var tokenResult = tokenService.Create();
            var access = JwtHelper.CreateAccessToken(user, tokenResult.Jti, jwtConfig.Value);

            return access;
        }

        public async Task<User?> GetUserBySecurityStampAsync(string stamp)
        {
            _log.Trace("GetUserBySecurityStampAsync called for stamp={Stamp}", stamp);

            var user = await unitOfWork.Users.GetBySecurityStamp(stamp, context.CancellationToken);

            if (user == null)
            {
                _log.Warn("User not found by securityStamp={Stamp}", stamp);
            }
            else
            {
                _log.Trace("User found id={Id} by securityStamp", user.Id);
            }

            return user;
        }

        public async Task<User> GetCurrentUserAsync(bool throwIfAnonymous)
        {
            if (throwIfAnonymous && (context.IsAnonymous || string.IsNullOrWhiteSpace(context.SecurityStamp)))
            {
                throw new AuthSecureException("Anonymous access is not allowed");
            }

            var user = await unitOfWork.Users.GetBySecurityStamp(context.SecurityStamp!, context.CancellationToken);

            if (user == null)
            {
                throw new NotFoundSecureException("User not found");
            }

            return user;
        }
    }
}
