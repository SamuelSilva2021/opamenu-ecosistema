using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.AccessGroup
{
    /// <summary>
    /// ServiÃ§o para gerenciamento de grupos de acesso
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="accessGroupRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="userContext"></param>
    public class AccessGroupService(
        ILogger<AccessGroupService> logger,
        IAccessGroupRepository accessGroupRepository,
        IMapper mapper,
        IUserContext userContext
        ) : IAccessGroupService
    {
        private readonly ILogger<AccessGroupService> _logger = logger;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserContext _userContext = userContext;

        /// <summary>
        /// Cria um novo grupo de acesso
        /// </summary>
        /// <param name="createAccessGroupDTO"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<AccessGroupDTO>> CreateAsync(CreateAccessGroupDTO createAccessGroupDTO)
        {
            try
            {
                var entity = _mapper.Map<AccessGroupEntity>(createAccessGroupDTO);
                var createdEntity = await _accessGroupRepository.AddAsync(entity);

                var dto = _mapper.Map<AccessGroupDTO>(createdEntity);
                return ResponseBuilder<AccessGroupDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar o grupo de acesso");
                return ResponseBuilder<AccessGroupDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
        /// <summary>
        /// Deleta um grupo de acesso por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _accessGroupRepository.GetByIdAsync(id);
                if (entity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Grupo de accesso nÃ£o encontrado." })
                        .WithCode(404)
                        .Build();

                await _accessGroupRepository.DeleteAsync(entity);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar grupo de acesso.");
                return ResponseBuilder<bool>.Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
        /// <summary>
        /// Recupera todos os grupos de acesso
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAllAsync()
        {
            try
            {
                var currentUser = _userContext.CurrentUser;
                if (currentUser == null)
                {
                    _logger.LogWarning("usuário nÃ£o autenticado ao tentar buscar grupos de acesso");
                    return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                        .Fail(new ErrorDTO { Message = "usuário nÃ£o autenticado" })
                        .WithCode(401)
                        .Build();
                }

                IEnumerable<AccessGroupEntity> entities;

                if (currentUser.TenantId == Guid.Empty || currentUser.TenantId == null)
                {
                    entities = await _accessGroupRepository.GetAllAsync();
                    if (entities == null || !entities.Any())
                        return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                            .Ok(Enumerable.Empty<AccessGroupDTO>())
                            .Build();
                    //_logger.LogWarning("TenantId nÃ£o encontrado para o usuário: {UserId}", currentUser.UserId);
                    //return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                    //    .Fail(new ErrorDTO { Message = "Tenant nÃ£o identificado" })
                    //    .WithCode(400)
                    //    .Build();
                }
                else
                {
                    // Filtra grupos de acesso pelo TenantId do usuário
                    entities = await _accessGroupRepository.GetAllAsyncByTenantId(currentUser.TenantId);
                    if (entities == null || !entities.Any())
                        return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                            .Ok(Enumerable.Empty<AccessGroupDTO>())
                            .Build();
                }

                var dto = _mapper.Map<IEnumerable<AccessGroupDTO>>(entities);
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar grupos de acesso");
                return ResponseBuilder<IEnumerable<AccessGroupDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }

        }
        /// <summary>
        /// Recupera um grupo de acesso por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<AccessGroupDTO>> GetByIdAsync(Guid id)
        {
            var entity = await _accessGroupRepository.GetByIdAsync(id);
            if (entity == null)
                return ResponseBuilder<AccessGroupDTO>
                    .Fail(new ErrorDTO { Message = "Grupo de accesso nÃ£o encontrado." }).WithCode(404).Build();

            var dto = _mapper.Map<AccessGroupDTO>(entity);
            return ResponseBuilder<AccessGroupDTO>.Ok(dto).Build();
        }
        /// <summary>
        /// Recupera grupos de acesso paginados
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PagedResponseDTO<AccessGroupDTO>>> GetPagedAsync(int page, int limit)
        {
            try
            {
                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var currentUser = _userContext.CurrentUser;

                PagedResponseDTO<AccessGroupDTO> pagedResult;
                int total;
                IEnumerable<AccessGroupEntity> entities;
                IEnumerable<AccessGroupDTO> items;

                if (currentUser.TenantId == Guid.Empty || currentUser.TenantId == null)
                {
                    total = await _accessGroupRepository.CountAsync();

                    entities = await _accessGroupRepository.GetPagedWithIncludesAsync(page, limit, ag => ag.GroupType);

                    items = _mapper.Map<IEnumerable<AccessGroupDTO>>(entities);

                    var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

                    pagedResult = new PagedResponseDTO<AccessGroupDTO>
                    {
                        Items = items,
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = totalPages
                    };

                }
                else
                {
                    // Filtra por tenant e inclui GroupType corretamente (Where + Include)
                    total = await _accessGroupRepository.CountAsync(ag => ag.TenantId == currentUser.TenantId);

                    entities = await _accessGroupRepository.GetPagedAsync(
                        page,
                        limit,
                        include: q => q
                            .Where(ag => ag.TenantId == currentUser.TenantId)
                            .Include(ag => ag.GroupType));

                    items = _mapper.Map<IEnumerable<AccessGroupDTO>>(entities);

                    var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

                    pagedResult = new PagedResponseDTO<AccessGroupDTO>
                    {
                        Items = items,
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = totalPages
                    };

                }
                return ResponseBuilder<PagedResponseDTO<AccessGroupDTO>>.Ok(pagedResult).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar grupos de acesso paginados");
                return ResponseBuilder<PagedResponseDTO<AccessGroupDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
        /// <summary>
        /// Atualiza um grupo de acesso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<AccessGroupDTO>> UpdateAsync(Guid id, UpdateAccessGroupDTO dto)
        {
            try
            {
                var entity = await _accessGroupRepository.GetByIdAsync(id);
                if (entity == null)
                    return ResponseBuilder<AccessGroupDTO>
                        .Fail(new ErrorDTO { Message = "Grupo de accesso nÃ£o encontrado." }).WithCode(404).Build();

                var updatedEntity = _mapper.Map(dto, entity);

                await _accessGroupRepository.UpdateAsync(updatedEntity);
                var updatedDto = _mapper.Map<AccessGroupDTO>(updatedEntity);

                return ResponseBuilder<AccessGroupDTO>.Ok(updatedDto).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o grupo de acesso.");
                return ResponseBuilder<AccessGroupDTO>.Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
    }
}

