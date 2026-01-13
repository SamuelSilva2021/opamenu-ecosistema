using Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations
{
    /// <summary>
    /// Interface para o serviço de relações Permissão-Operação
    /// </summary>
    public interface IPermissionOperationService
    {
        /// <summary>
        /// Obtém todas as relações Permissão-Operação
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetAllPermissionOperationsAsync();

        /// <summary>
        /// Obtém relações por ID da permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Obtém relações por ID da operação
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByOperationIdAsync(Guid operationId);

        /// <summary>
        /// Obtém uma relação específica entre permissão e operação
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionOperationDTO>> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId);

        /// <summary>
        /// Cria uma nova relação Permissão-Operação
        /// </summary>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionOperationDTO>> CreatePermissionOperationAsync(PermissionOperationCreateDTO permissionOperation);

        /// <summary>
        /// Cria múltiplas relações Permissão-Operação
        /// </summary>
        /// <param name="permissionOperations"></param>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> CreatePermissionOperationsBulkAsync(PermissionOperationBulkDTO permissionOperations);

        /// <summary>
        /// Atualiza uma relação Permissão-Operação
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionOperationDTO>> UpdatePermissionOperationAsync(Guid id, PermissionOperationUpdateDTO permissionOperation);

        /// <summary>
        /// Remove uma relação Permissão-Operação (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeletePermissionOperationAsync(Guid id);

        /// <summary>
        /// Remove todas as relações de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteAllByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Remove relações específicas de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds);
    }
}