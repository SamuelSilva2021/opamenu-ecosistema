using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.RoleAccessGroups
{
    /// <summary>
    /// Repositório para relações Role-AccessGroup
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class RoleAccessGroupRepository(
        IDbContextProvider dbContextProvider,
        ILogger<RoleAccessGroupRepository> logger
    ) : BaseRepository<RoleAccessGroupEntity>(dbContextProvider), IRoleAccessGroupRepository
    {
        private readonly ILogger<RoleAccessGroupRepository> _logger = logger;

        public async Task<IEnumerable<RoleAccessGroupEntity>> GetByRoleIdAsync(Guid roleId)
        {
            try
            {
                return await GetAllAsync(
                    filter: rag => rag.RoleId == roleId && rag.IsActive,
                    include: query => query
                        .Include(rag => rag.Role)
                        .Include(rag => rag.AccessGroup)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relações por role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<IEnumerable<RoleAccessGroupEntity>> GetByAccessGroupIdAsync(Guid accessGroupId)
        {
            try
            {
                return await GetAllAsync(
                    filter: rag => rag.AccessGroupId == accessGroupId && rag.IsActive,
                    include: query => query
                        .Include(rag => rag.Role)
                        .Include(rag => rag.AccessGroup)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relações por grupo de acesso {AccessGroupId}", accessGroupId);
                throw;
            }
        }

        public async Task<RoleAccessGroupEntity?> GetByRoleAndGroupAsync(Guid roleId, Guid accessGroupId)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: rag => rag.RoleId == roleId && rag.AccessGroupId == accessGroupId && rag.IsActive,
                    include: query => query
                        .Include(rag => rag.Role)
                        .Include(rag => rag.AccessGroup)
                );
                return relations.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relação entre role {RoleId} e grupo {AccessGroupId}", roleId, accessGroupId);
                throw;
            }
        }

        public async Task<bool> RemoveAllByRoleIdAsync(Guid roleId)
        {
            try
            {
                var relations = await GetAllAsync(rag => rag.RoleId == roleId && rag.IsActive);
                foreach (var relation in relations)
                {
                    relation.IsActive = false;
                    relation.UpdatedAt = DateTime.Now;
                    await UpdateAsync(relation);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover todas as relações do role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> RemoveByRoleAndGroupsAsync(Guid roleId, IEnumerable<Guid> groupIds)
        {
            try
            {
                var relations = await GetAllAsync(rag => rag.RoleId == roleId && groupIds.Contains(rag.AccessGroupId) && rag.IsActive);
                foreach (var relation in relations)
                {
                    relation.IsActive = false;
                    relation.UpdatedAt = DateTime.Now;
                    await UpdateAsync(relation);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover relações específicas do role {RoleId}", roleId);
                throw;
            }
        }
    }
}
