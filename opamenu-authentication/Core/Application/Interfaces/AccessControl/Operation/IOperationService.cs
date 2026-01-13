using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Operation
{
    /// <summary>
    /// Interface para o serviço de operações
    /// </summary>
    public interface IOperationService
    {
        /// <summary>
        /// Obtém todas as operações
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<OperationDTO>>> GetAllOperationAsync();
        /// <summary>
        /// Obtém todas as operações paginadas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<OperationDTO>>> GetAllOperationPagedAsync(int page, int limit);
        /// <summary>
        /// Obtém uma operação pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<OperationDTO>> GetOperationByIdAsync(Guid id);
        /// <summary>
        /// Adiciona uma nova operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task<ResponseDTO<OperationDTO>> AddOperationAsync(OperationCreateDTO operation);
        /// <summary>
        /// Atualiza uma operação
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task<ResponseDTO<OperationDTO>> UpdateOperationAsync(Guid id, OperationUpdateDTO operation);
        /// <summary>
        /// Deleta uma operação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteOperationAsync(Guid id);
    }
}
