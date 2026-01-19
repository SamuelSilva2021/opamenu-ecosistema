using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using AutoMapper;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Operation
{
    /// <summary>
    /// Serviço para gerenciar operações
    /// </summary>
    /// <param name="operationRepository"></param>
    /// <param name="mapper"></param>
    public class OperationService(IOperationRepository operationRepository, IMapper mapper, IUserContext userContext) : IOperationService
    {
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserContext _userContext = userContext;
        /// <summary>
        /// Adiciona uma nova operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<OperationDTO>> AddOperationAsync(OperationCreateDTO operation)
        {
            try
            {
                var entity = _mapper.Map<OperationEntity>(operation);
                var valueExist = await _operationRepository.ExisteValue(operation.Value!);

                if (valueExist)
                    return StaticResponseBuilder<OperationDTO>.BuildError("Já existe uma operação com esse valor.");

                var createdEntity = await _operationRepository.AddAsync(entity);
                var dto = _mapper.Map<OperationDTO>(createdEntity);

                return StaticResponseBuilder<OperationDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<OperationDTO>.BuildErrorResponse(ex);
            }
            
        }
        /// <summary>
        /// Deleta uma operação por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteOperationAsync(Guid id)
        {
            try
            {
                var existingEntity = await _operationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return StaticResponseBuilder<bool>.BuildError("Operação não encontrada");

                await _operationRepository.DeleteAsync(existingEntity);
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }
        /// <summary>
        /// Obtém todas as operações
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<OperationDTO>>> GetAllOperationAsync()
        {
            try
            {
                var entities = await _operationRepository.GetAllAsync();
                if (entities == null)
                    return StaticResponseBuilder<IEnumerable<OperationDTO>>.BuildOk([]);

                var dtos = _mapper.Map<IEnumerable<OperationDTO>>(entities);
                return StaticResponseBuilder<IEnumerable<OperationDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<OperationDTO>>.BuildErrorResponse(ex);
            }
        }
        /// <summary>
        /// Obtém todas as operações paginadas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseDTO<PagedResponseDTO<OperationDTO>>> GetAllOperationPagedAsync(int page, int limit)
        {
            try
            {
                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var currentUser = _userContext.CurrentUser;


                var total = await _operationRepository.CountAsync();

                var entities = await _operationRepository.GetPagedWithIncludesAsync(page, limit, o => o.PermissionOperations);

                var items = _mapper.Map<IEnumerable<OperationDTO>>(entities);

                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

                var pagedResult = new PagedResponseDTO<OperationDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };
                return StaticResponseBuilder<PagedResponseDTO<OperationDTO>>.BuildOk(pagedResult);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PagedResponseDTO<OperationDTO>>.BuildErrorResponse(ex);
            }
        }
        /// <summary>
        /// Obtém uma operação por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<OperationDTO>> GetOperationByIdAsync(Guid id)
        {
            try
            {
                var operationEntity = await _operationRepository.GetByIdAsync(id);
                if (operationEntity == null)
                    return StaticResponseBuilder<OperationDTO>.BuildOk(null!);

                var operationDTO = _mapper.Map<OperationDTO>(operationEntity);

                return StaticResponseBuilder<OperationDTO>.BuildOk(operationDTO);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<OperationDTO>.BuildErrorResponse(ex);
            }
        }
        /// <summary>
        /// Atualiza uma operação por ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<OperationDTO>> UpdateOperationAsync(Guid id, OperationUpdateDTO operation)
        {
            try
            {
                var existingEntity = await _operationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return StaticResponseBuilder<OperationDTO>.BuildError("Operação não encontrada");

                existingEntity = _mapper.Map(operation, existingEntity);

                await _operationRepository.UpdateAsync(existingEntity);

                var dto = _mapper.Map<OperationDTO>(existingEntity);
                return StaticResponseBuilder<OperationDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<OperationDTO>.BuildErrorResponse(ex);
            }
        }
    }
}

