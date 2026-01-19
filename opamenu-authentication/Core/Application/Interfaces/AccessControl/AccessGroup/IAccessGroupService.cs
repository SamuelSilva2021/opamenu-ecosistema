using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    /// <summary>
    /// Serviço de grupo de acesso
    /// </summary>
    public interface IAccessGroupService
    {
        /// <summary>
        /// Obtém todos os grupos de acesso
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAllAsync();
        /// <summary>
        /// Obtém grupos de acesso paginados
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<AccessGroupDTO>>> GetPagedAsync(int page, int limit);
        /// <summary>
        /// Obtém grupo de acesso por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<AccessGroupDTO>> GetByIdAsync(Guid id);
        /// <summary>
        /// Cria um novo grupo de acesso
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResponseDTO<AccessGroupDTO>> CreateAsync(CreateAccessGroupDTO dto);
        /// <summary>
        /// Atualiza um grupo de acesso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResponseDTO<AccessGroupDTO>> UpdateAsync(Guid id, UpdateAccessGroupDTO dto);
        /// <summary>
        /// Atualiza o status do grupo de acesso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResponseDTO<AccessGroupDTO>> ToggleStatus(Guid id, UpdateAccessGroupDTO dto);
        /// <summary>
        /// Deleta um grupo de acesso
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
