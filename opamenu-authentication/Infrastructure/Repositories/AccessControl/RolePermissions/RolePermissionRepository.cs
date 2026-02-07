using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.RolePermissions
{
    /// <summary>
    /// Repositório para relações Role-Permission
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class RolePermissionRepository(
        IDbContextProvider dbContextProvider,
        ILogger<RolePermissionRepository> logger
    ) : BaseRepository<RolePermissionEntity>(dbContextProvider), IRolePermissionRepository
    {
        private readonly ILogger<RolePermissionRepository> _logger = logger;

        public async Task<IEnumerable<RolePermissionEntity>> GetAllRolePermissionsByRoleIdAsync(Guid roleId)
        {
            try
            {
                return await GetAllAsync(
                    filter: rp => rp.RoleId == roleId,
                    include: query => query.Include(rp => rp.Role)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relações por role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<RolePermissionEntity?> GetByRoleAndModuleAsync(Guid roleId, string moduleKey)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: rp => rp.RoleId == roleId && rp.ModuleKey == moduleKey && rp.IsActive,
                    include: query => query.Include(rp => rp.Role)
                );
                return relations.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relação entre role {RoleId} e módulo {ModuleKey}", roleId, moduleKey);
                throw;
            }
        }

        public async Task<bool> RemoveAllByRoleIdAsync(Guid roleId)
        {
            try
            {
                var relations = await GetAllAsync(rp => rp.RoleId == roleId && rp.IsActive);
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

        public async Task<bool> RemoveByRoleAndModulesAsync(Guid roleId, IEnumerable<string> moduleKeys)
        {
            try
            {
                var relations = await GetAllAsync(rp => rp.RoleId == roleId && moduleKeys.Contains(rp.ModuleKey) && rp.IsActive);
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
                _logger.LogError(ex, "Erro ao remover relações específicas por módulo do role {RoleId}", roleId);
                throw;
            }
        }
    }
}
