using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.PermissionOperations
{
    /// <summary>
    /// RepositÃ³rio para relaÃ§Ãµes PermissÃ£o-OperaÃ§Ã£o
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class PermissionOperationRepository(
        IDbContextProvider dbContextProvider,
        ILogger<PermissionOperationRepository> logger
        ) : BaseRepository<PermissionOperationEntity>(dbContextProvider), IPermissionOperationRepository
    {
        private readonly ILogger<PermissionOperationRepository> _logger = logger;

        /// <summary>
        /// Busca relaÃ§Ãµes por ID da permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PermissionOperationEntity>> GetByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                return await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && po.IsActive,
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relaÃ§Ãµes por permissÃ£o {PermissionId}", permissionId);
                throw;
            }
        }

        /// <summary>
        /// Busca relaÃ§Ãµes por ID da operaÃ§Ã£o
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PermissionOperationEntity>> GetByOperationIdAsync(Guid operationId)
        {
            try
            {
                return await GetAllAsync(
                    filter: po => po.OperationId == operationId && po.IsActive,
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relaÃ§Ãµes por operaÃ§Ã£o {OperationId}", operationId);
                throw;
            }
        }

        /// <summary>
        /// Busca uma relaÃ§Ã£o especÃ­fica entre permissÃ£o e operaÃ§Ã£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<PermissionOperationEntity?> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && 
                                  po.OperationId == operationId && 
                                  po.IsActive, 
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );

                return relations.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relaÃ§Ã£o entre permissÃ£o {PermissionId} e operaÃ§Ã£o {OperationId}", 
                    permissionId, operationId);
                throw;
            }
        }

        /// <summary>
        /// Remove todas as relaÃ§Ãµes de uma permissÃ£o (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAllByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId 
                        && po.IsActive
                );

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
                _logger.LogError(ex, "Erro ao remover todas as relaÃ§Ãµes da permissÃ£o {PermissionId}", permissionId);
                throw;
            }
        }

        /// <summary>
        /// Remove relaÃ§Ãµes especÃ­ficas (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<bool> RemoveByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && 
                                  operationIds.Contains(po.OperationId) && 
                                  po.IsActive
                );

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
                _logger.LogError(ex, "Erro ao remover relaÃ§Ãµes especÃ­ficas da permissÃ£o {PermissionId}", permissionId);
                throw;
            }
        }
    }
}
