using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions
{
    /// <summary>
    /// Interface para o serviço de permissões
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Obtém todas as permissões
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync();

        /// <summary>
        /// Obtém todas as permissões paginadas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<PermissionDTO>>> GetAllPermissionsPagedAsync(int page, int limit);

        /// <summary>
        /// Obtém uma permissão pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionDTO>> GetPermissionByIdAsync(Guid id);

        /// <summary>
        /// Obtém permissões por módulo
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByModuleAsync(Guid moduleId);

        /// <summary>
        /// Obtém permissões por role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId);

        /// <summary>
        /// Adiciona uma nova permissão
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionDTO>> AddPermissionAsync(PermissionCreateDTO permission);

        /// <summary>
        /// Atualiza uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionDTO>> UpdatePermissionAsync(Guid id, PermissionUpdateDTO permission);

        /// <summary>
        /// Alterna o status ativo/inativo de uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<PermissionDTO>> ToggleStatus(Guid id);

        /// <summary>
        /// Deleta uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeletePermissionAsync(Guid id);

        /// <summary>
        /// Associa operações a uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> AssignOperationsToPermissionAsync(Guid permissionId, List<Guid> operationIds);

        /// <summary>
        /// Remove operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> RemoveOperationsFromPermissionAsync(Guid permissionId, List<Guid> operationIds);
    }
}
