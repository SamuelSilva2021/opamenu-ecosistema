using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations
{
    /// <summary>
    /// Interface do repositÃ³rio de relaÃ§Ãµes PermissÃ£o-OperaÃ§Ã£o
    /// </summary>
    public interface IPermissionOperationRepository : IBaseRepository<PermissionOperationEntity>
    {
        /// <summary>
        /// Busca relaÃ§Ãµes por ID da permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionOperationEntity>> GetByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Busca relaÃ§Ãµes por ID da operaÃ§Ã£o
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionOperationEntity>> GetByOperationIdAsync(Guid operationId);

        /// <summary>
        /// Busca uma relaÃ§Ã£o especÃ­fica entre permissÃ£o e operaÃ§Ã£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<PermissionOperationEntity?> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId);

        /// <summary>
        /// Remove todas as relaÃ§Ãµes de uma permissÃ£o (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<bool> RemoveAllByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Remove relaÃ§Ãµes especÃ­ficas (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        Task<bool> RemoveByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds);
    }
}
