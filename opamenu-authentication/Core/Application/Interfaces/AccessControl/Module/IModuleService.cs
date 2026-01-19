using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Module
{
    /// <summary>
    /// Serviço para gerenciamento de módulos
    /// </summary>
    public interface IModuleService
    {
        /// <summary>
        /// Obtém todos os módulos
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<ModuleDTO>>> GetAllModuleAsync();
        /// <summary>
        /// Obtém todos os módulos paginados
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<ModuleDTO>>> GetAllModulePagedAsync(int page, int limit);
        /// <summary>
        /// Obtém um módulo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<ModuleDTO>> GetModuleByIdAsync(Guid id);
        /// <summary>
        /// Adiciona um novo módulo
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        Task<ResponseDTO<ModuleDTO>> AddModuleAsync(ModuleCreateDTO moduleType);
        /// <summary>
        /// Atualiza um módulo existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        Task<ResponseDTO<ModuleDTO>> UpdateModuleAsync(Guid id, ModuleUpdateDTO moduleType);
        Task<ResponseDTO<ModuleDTO>> ToggleStatus(Guid id);
        /// <summary>
        /// Remove um módulo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteModuleAsync(Guid id);
    }
}
