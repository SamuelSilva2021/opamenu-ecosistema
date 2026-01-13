using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Module
{
    /// <summary>
    /// Serviço para gerenciar operações relacionadas a módulos
    /// </summary>
    /// <param name="moduleRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="userContext"></param>
    public class ModuleService(
        IModuleRepository moduleRepository,
        IMapper mapper,
        IUserContext userContext
        ) : IModuleService
    {
        private readonly IModuleRepository _moduleRepository = moduleRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserContext _userContext = userContext;
        /// <summary>
        /// Adiciona um novo módulo
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<ModuleDTO>> AddModuleAsync(ModuleCreateDTO module)
        {
            try
            {
                var moduleEntity = _mapper.Map<ModuleEntity>(module);

                var createdModuleType = await _moduleRepository.AddAsync(moduleEntity);

                var moduleTypeDTO = _mapper.Map<ModuleDTO>(createdModuleType);

                return ResponseBuilder<ModuleDTO>.Ok(moduleTypeDTO).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
        /// <summary>
        /// Deleta um módulo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteModuleAsync(Guid id)
        {
            try
            {
                var existingModuleType = await _moduleRepository.GetByIdAsync(id);
                if (existingModuleType == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Modulo não encontrado" })
                        .WithCode(404)
                        .Build();
                await _moduleRepository.DeleteAsync(existingModuleType);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
        /// <summary>
        /// Obtém todos os módulos
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<ModuleDTO>>> GetAllModuleAsync()
        {
            try
            {
                var moduleTypes = await _moduleRepository.GetAllAsync();

                if(!moduleTypes.Any())
                    return ResponseBuilder<IEnumerable<ModuleDTO>>.Ok([]).Build();

                var moduleTypesDTO = _mapper.Map<IEnumerable<ModuleDTO>>(moduleTypes);

                return ResponseBuilder<IEnumerable<ModuleDTO>>.Ok(moduleTypesDTO).Build();

            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<ModuleDTO>>
                    .Fail(new ErrorDTO{Message = ex.Message})
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
        /// <summary>
        /// Obtém módulos paginados
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PagedResponseDTO<ModuleDTO>>> GetAllModulePagedAsync(int page, int limit)
        {
            try
            {
                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var currentUser = _userContext.CurrentUser;
                var entities = await _moduleRepository.GetPagedWithIncludesAsync(page, limit, m => m.Permissions);
                var total = entities.Count();

                var items = _mapper.Map<IEnumerable<ModuleDTO>>(entities);

                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);
                var pagedResult = new PagedResponseDTO<ModuleDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };

                return ResponseBuilder<PagedResponseDTO<ModuleDTO>>.Ok(pagedResult).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PagedResponseDTO<ModuleDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
            
        }
        /// <summary>
        /// Obtém um módulo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<ModuleDTO>> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var moduleType = await _moduleRepository.GetByIdAsync(id);
                if (moduleType == null)
                    return ResponseBuilder<ModuleDTO>
                        .Fail(new ErrorDTO { Message = "Module type not found." })
                        .WithCode(404)
                        .Build();

                var moduleTypeDTO = _mapper.Map<ModuleDTO>(moduleType);
                return ResponseBuilder<ModuleDTO>.Ok(moduleTypeDTO).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
        /// <summary>
        /// Atualiza um módulo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<ModuleDTO>> UpdateModuleAsync(Guid id, ModuleUpdateDTO moduleType)
        {
            try
            {
                var existingModuleType = await _moduleRepository.GetByIdAsync(id);
                if (existingModuleType == null)
                    return ResponseBuilder<ModuleDTO>
                        .Fail(new ErrorDTO { Message = "Modulo não encotrado" })
                        .WithCode(404)
                        .Build();

                var moduleUpdatedEntity = _mapper.Map(moduleType, existingModuleType);

                await _moduleRepository.UpdateAsync(moduleUpdatedEntity);

                var moduleTypeDTO = _mapper.Map<ModuleDTO>(moduleUpdatedEntity);
                return ResponseBuilder<ModuleDTO>.Ok(moduleTypeDTO).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
    }
}

