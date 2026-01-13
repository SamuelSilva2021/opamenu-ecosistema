using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Infrastructure.Data.Context;
using Authenticator.API.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.AccountAccessGroups
{
    /// <summary>
    /// RepositÃ³rio para vÃ­nculos de grupos de acesso com usuários
    /// </summary>
    public class AccountAccessGroupRepository : BaseRepository<AccountAccessGroupEntity>, IAccountAccessGroupRepository
    {
        private readonly IDbContextProvider _dbContextProvider;

        public AccountAccessGroupRepository(IDbContextProvider dbContextProvider) : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<IEnumerable<AccountAccessGroupEntity>> GetByUserAsync(Guid userId)
        {
            var context = (AccessControlDbContext)_dbContextProvider.GetContext<AccountAccessGroupEntity>();
            return await context.AccountAccessGroups
                .Where(aag => aag.UserAccountId == userId && aag.IsActive)
                .Include(aag => aag.AccessGroup)
                .ToListAsync();
        }

        public async Task AssignGroupsAsync(Guid userId, IEnumerable<Guid> accessGroupIds, Guid? grantedBy, DateTime? expiresAt)
        {
            var context = (AccessControlDbContext)_dbContextProvider.GetContext<AccountAccessGroupEntity>();
            var dbSet = context.AccountAccessGroups;

            foreach (var groupId in accessGroupIds.Distinct())
            {
                var existing = await dbSet.FirstOrDefaultAsync(x => x.UserAccountId == userId && x.AccessGroupId == groupId);
                if (existing == null)
                {
                    var entity = new AccountAccessGroupEntity
                    {
                        Id = Guid.NewGuid(),
                        UserAccountId = userId,
                        AccessGroupId = groupId,
                        IsActive = true,
                        GrantedBy = grantedBy,
                        GrantedAt = DateTime.UtcNow,
                        ExpiresAt = expiresAt,
                        CreatedAt = DateTime.UtcNow
                    };

                    await dbSet.AddAsync(entity);
                }
                else if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    existing.GrantedBy = grantedBy;
                    existing.GrantedAt = DateTime.UtcNow;
                    existing.ExpiresAt = expiresAt;
                    existing.UpdatedAt = DateTime.UtcNow;

                    dbSet.Update(existing);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> RevokeAsync(Guid userId, Guid accessGroupId)
        {
            var context = (AccessControlDbContext)_dbContextProvider.GetContext<AccountAccessGroupEntity>();
            var existing = await context.AccountAccessGroups.FirstOrDefaultAsync(x => x.UserAccountId == userId && x.AccessGroupId == accessGroupId);
            if (existing == null || !existing.IsActive)
                return false;

            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            context.AccountAccessGroups.Update(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
