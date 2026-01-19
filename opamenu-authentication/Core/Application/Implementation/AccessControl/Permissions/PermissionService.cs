using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Xunit.Sdk;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Permissions
{
    /// <summary>
    /// Serviço para gerenciar permissões
    /// </summary>
    /// <param name="permissionRepository"></param>
    /// <param name="operationRepository"></param>
    /// <param name="mapper"></param>
    public class PermissionService(
        IPermissionRepository permissionRepository,
        IOperationRepository operationRepository,
        IPermissionOperationRepository permissionOperationRepository,
        IMapper mapper) : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IPermissionOperationRepository _permissionOperationRepository = permissionOperationRepository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obtém todas as permissões
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync()
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(include: p => p
                    .Include(p => p.RolePermissions)
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                if(!entities.Any())
                    return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(null!);

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Obtém todas as permissões paginadas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PagedResponseDTO<PermissionDTO>>> GetAllPermissionsPagedAsync(int page, int limit)
        {
            try
            {
                var entities = await _permissionRepository.GetPagedAsync(
                    page: page,
                    pageSize: limit,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                if(!entities.Any())
                    return StaticResponseBuilder<PagedResponseDTO<PermissionDTO>>.BuildOk(null!);

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);

                var pagedResponse = new PagedResponseDTO<PermissionDTO>
                {
                    Items = dtos,
                    Page = page,
                    Limit = limit,
                    Total = entities.Count(),
                    TotalPages = (int)Math.Ceiling((double)entities.Count() / limit)
                };
                return StaticResponseBuilder<PagedResponseDTO<PermissionDTO>>.BuildOk(pagedResponse);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PagedResponseDTO<PermissionDTO>>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Obtém permissão por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                var entity = await _permissionRepository.GetByIdAsync(id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                if (entity == null)
                    return StaticResponseBuilder<PermissionDTO>.BuildOk(null!);

                var dto = _mapper.Map<PermissionDTO>(entity);
                return StaticResponseBuilder<PermissionDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PermissionDTO>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Obtem permissões por módulo
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByModuleAsync(Guid moduleId)
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(
                    filter: p => p.ModuleId == moduleId,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                if(entities == null)
                    return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(null!);

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Obtém permissão por role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId)
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(
                    filter: p => p.RolePermissions.Any(rp => rp.RoleId == roleId && rp.IsActive) && p.IsActive,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                if(entities == null)
                    return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(null!);

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<PermissionDTO>>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Adiciona uma nova permissão
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> AddPermissionAsync(PermissionCreateDTO permission)
        {
            try
            {
                var entity = _mapper.Map<PermissionEntity>(permission);
                entity.CreatedAt = DateTime.Now;

                var createdEntity = await _permissionRepository.AddAsync(entity);

                // Associar operações se fornecidas
                if (permission.OperationIds != null && permission.OperationIds.Any())
                {
                    await AssignOperationsToPermissionInternalAsync(createdEntity.Id, permission.OperationIds);
                }

                // Recarregar a entidade com includes
                var entityWithIncludes = await _permissionRepository.GetByIdAsync(createdEntity.Id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                var dto = _mapper.Map<PermissionDTO>(entityWithIncludes);
                return StaticResponseBuilder<PermissionDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PermissionDTO>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Atualiza uma permissão existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> UpdatePermissionAsync(Guid id, PermissionUpdateDTO permission)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                if (existingEntity == null)
                    return StaticResponseBuilder<PermissionDTO>.BuildError("Permissão não encontrada!");

                _mapper.Map(permission, existingEntity);
                await _permissionRepository.UpdateAsync(existingEntity);

                var operationsChanges = PermissionsOperationsCompare(existingEntity.PermissionOperations, permission);

                if (operationsChanges.Count > 0)
                {
                    foreach (var change in operationsChanges)
                    {
                        var operationId = change.Keys.First();
                        var action = change.Values.First();
                        await AddOrRemoveOperationsFromPermissionInternal(id, operationId , existingEntity.PermissionOperations, action);
                    }
                    await _permissionOperationRepository.UpdateRangeAsync(existingEntity.PermissionOperations);
                }

                var dto = _mapper.Map<PermissionDTO>(existingEntity);
                return StaticResponseBuilder<PermissionDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PermissionDTO>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Deleta uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeletePermissionAsync(Guid id)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return StaticResponseBuilder<bool>.BuildError("Permissão não encontrada!");

                // Remover todos o vinculos associadas antes de deletar
                await RemoveAllOperationsFromPermissionInternalAsync(id);

                await _permissionRepository.DeleteAsync(existingEntity);
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Associa operações a uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> AssignOperationsToPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var result = await AssignOperationsToPermissionInternalAsync(permissionId, operationIds);
                return StaticResponseBuilder<bool>.BuildOk(result);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Remove vinculos de operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> RemoveOperationsFromPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId, include: p => p
                    .Include(x => x.PermissionOperations)) ?? throw new ArgumentException("Permissão não encontrada");

                var permissionOperationsToRemove = permission.PermissionOperations
                    .Where(po => operationIds.Contains(po.OperationId) && po.IsActive)
                    .ToList();

                if(permissionOperationsToRemove.Count == 0)
                    throw new ArgumentException("Nenhum vínculo de operação encontrado para remoção");

                foreach (var permissionOperation in permissionOperationsToRemove)
                {
                    permissionOperation.IsActive = false;
                    permissionOperation.UpdatedAt = DateTime.Now;
                }
                await _permissionOperationRepository.UpdateRangeAsync(permissionOperationsToRemove);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        /// <summary>
        /// Método interno para associar operações a uma permissão
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        private async Task<bool> AssignOperationsToPermissionInternalAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    throw new ArgumentException("PermissÃ£o nÃ£o encontrada");

                foreach (var operationId in operationIds)
                {
                    var operation = await _operationRepository.GetByIdAsync(operationId);
                    if (operation == null)
                        throw new ArgumentException($"OperaÃ§Ã£o com ID {operationId} nÃ£o encontrada");
                }

                var existingPermissionOperations = await _permissionRepository.GetAllAsync(
                    filter: po => po.Id == permissionId,
                    include: p => p.Include(x => x.PermissionOperations));

                var existingOperationIds = existingPermissionOperations
                    .SelectMany(p => p.PermissionOperations)
                    .Where(po => po.IsActive)
                    .Select(po => po.OperationId)
                    .ToList();

                var newOperationIds = operationIds.Except(existingOperationIds).ToList();

                List<PermissionOperationEntity> permissionOperations = new List<PermissionOperationEntity>();

                foreach (var operationId in newOperationIds)
                {
                    var permissionOperation = new PermissionOperationEntity
                    {
                        Id = Guid.NewGuid(),
                        PermissionId = permissionId,
                        OperationId = operationId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    permissionOperations.Add(permissionOperation);

                }
                await _permissionOperationRepository.AddRangeAsync(permissionOperations);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Método interno para adicionar ou remover operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <param name="permissionOperations"></param>
        /// <returns></returns>
        private async Task<IEnumerable<PermissionOperationEntity>> AddOrRemoveOperationsFromPermissionInternal(Guid permissionId, Guid operationId,
            IEnumerable<PermissionOperationEntity> permissionOperations, string action)
        {
            try
            {
                if (action == "add")
                {
                    var existingPermissionOperation = permissionOperations.FirstOrDefault(po => po.PermissionId == permissionId && po.OperationId == operationId);

                    if (existingPermissionOperation != null)
                    {
                        if (!existingPermissionOperation.IsActive)
                        {
                            existingPermissionOperation.IsActive = true;
                            existingPermissionOperation.UpdatedAt = DateTime.Now;
                        }
                    }
                    else
                    {
                        var permissionOperation = new PermissionOperationEntity
                        {
                            Id = Guid.NewGuid(),
                            PermissionId = permissionId,
                            OperationId = operationId,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        };
                        await _permissionOperationRepository.AddAsync(permissionOperation);

                        permissionOperations.ToList().Add(permissionOperation);
                    }
                    return permissionOperations;
                }
                else if (action == "remove")
                {
                    var existingPermissionOperation = permissionOperations.FirstOrDefault(po => po.PermissionId == permissionId && po.OperationId == operationId);
                    if (existingPermissionOperation != null && existingPermissionOperation.IsActive)
                    {
                        existingPermissionOperation.IsActive = false;
                        existingPermissionOperation.UpdatedAt = DateTime.Now;
                    }
                    return permissionOperations;
                }
                return permissionOperations;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar ou remover operações da permissão.", ex);
            }

        }

        /// <summary>
        /// Método interno para remover todas as operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        private async Task<bool> RemoveAllOperationsFromPermissionInternalAsync(Guid permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId, include: p => p
                .Include(x => x.PermissionOperations));

            if (permission == null)
                return false;
            
            await _permissionOperationRepository.DeleteRangeAsync(permission.PermissionOperations);

            return true;

        }

        /// <summary>
        /// Compara as operações de uma permissão existente com as novas operações fornecidas
        /// </summary>
        /// <param name="permissionOperations"></param>
        /// <param name="newPermission"></param>
        /// <returns></returns>
        private static List<Dictionary<Guid, string>> PermissionsOperationsCompare(IEnumerable<PermissionOperationEntity> permissionOperations, PermissionUpdateDTO newPermission)
        {
            List<Dictionary<Guid, string>> operationsCompare = new List<Dictionary<Guid, string>>();

            foreach (var op in newPermission.OperationIds!)
            {
                if (!permissionOperations.Any(po => po.OperationId == op && po.IsActive))
                {
                    operationsCompare.Add(new Dictionary<Guid, string> { { op, "add" } });
                }
            }
            foreach (var op in permissionOperations)
            {
                if (!newPermission.OperationIds.Any(po => po == op.OperationId) && op.IsActive)
                {
                    operationsCompare.Add(new Dictionary<Guid, string> { { op.OperationId, "remove" } });
                }
            }
            return operationsCompare;
        }

        public async Task<ResponseDTO<PermissionDTO>> ToggleStatus(Guid id)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                if (existingEntity == null)
                    return StaticResponseBuilder<PermissionDTO>.BuildError("Permissão não encontrada!");

                existingEntity.IsActive = !existingEntity.IsActive;
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionRepository.UpdateAsync(existingEntity);

                var dto = _mapper.Map<PermissionDTO>(existingEntity);
                return StaticResponseBuilder<PermissionDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PermissionDTO>.BuildErrorResponse(ex);
            }
        }
    }
}
