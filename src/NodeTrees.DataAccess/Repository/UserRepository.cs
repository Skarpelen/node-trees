using Microsoft.EntityFrameworkCore;

namespace NodeTrees.DataAccess.Repository
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.DataAccess.Models;

    public class UserRepository(NodeContext context)
        : GenericRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetByCode(string code, CancellationToken ct)
        {
            var query = context.Users
                .Where(u => !u.IsDeleted && u.Code == code)
                .AsNoTracking();

            var user = await query.FirstOrDefaultAsync(ct);
            return user;
        }

        public async Task<User?> GetBySecurityStamp(string stamp, CancellationToken ct)
        {
            var query = context.Users
                .Where(u => !u.IsDeleted && u.SecurityStamp == stamp)
                .AsNoTracking();

            var user = await query.FirstOrDefaultAsync(ct);
            return user;
        }
    }
}
