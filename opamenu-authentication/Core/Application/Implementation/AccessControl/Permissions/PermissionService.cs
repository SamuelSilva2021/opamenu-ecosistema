using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Permissions
{
    /// <summary>
    /// ServiÃ§o para gerenciar permissÃµes
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
        /// ObtÃ©m todas as permissÃµes
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync()
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m todas as permissÃµes paginadas
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

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);

                var pagedResponse = new PagedResponseDTO<PermissionDTO>
                {
                    Items = dtos,
                    Page = page,
                    Limit = limit,
                    Total = entities.Count(),
                    TotalPages = (int)Math.Ceiling((double)entities.Count() / limit)
                };

                return ResponseBuilder<PagedResponseDTO<PermissionDTO>>.Ok(pagedResponse).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PagedResponseDTO<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m uma permissÃ£o pelo ID
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
                    return ResponseBuilder<PermissionDTO>
                        .Fail(new ErrorDTO { Message = "PermissÃ£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                var dto = _mapper.Map<PermissionDTO>(entity);
                return ResponseBuilder<PermissionDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m permissÃµes por mÃ³dulo
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

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m permissÃµes por role
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

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Adiciona uma nova permissÃ£o
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

                // Associar operaÃ§Ãµes se fornecidas
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
                return ResponseBuilder<PermissionDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Atualiza uma permissÃ£o
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
                    return ResponseBuilder<PermissionDTO>
                        .Fail(new ErrorDTO { Message = "PermissÃ£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                _mapper.Map(permission, existingEntity);
                existingEntity.UpdatedAt = DateTime.Now;
                await _permissionRepository.UpdateAsync(existingEntity);


                var operationsChanges = PermissionsOperationsCompare(existingEntity.PermissionOperations, permission);

                if (operationsChanges.Count > 0)
                {
                    foreach (var change in operationsChanges)
                    {
                        var operationId = change.Keys.First();
                        var action = change.Values.First();
                        if (action == "add")
                            await AddOrRemoveOperationsFromPermissionInternal(id, new List<Guid> { operationId }, existingEntity.PermissionOperations);
                        else if (action == "remove")
                            await AddOrRemoveOperationsFromPermissionInternal(id, new List<Guid> { operationId }, existingEntity.PermissionOperations);
                    }
                    await _permissionOperationRepository.UpdateRangeAsync(existingEntity.PermissionOperations);
                }

                var dto = _mapper.Map<PermissionDTO>(existingEntity);
                return ResponseBuilder<PermissionDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Deleta uma permissÃ£o
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeletePermissionAsync(Guid id)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "PermissÃ£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                // Remover todas as operaÃ§Ãµes associadas antes de deletar
                await RemoveAllOperationsFromPermissionInternalAsync(id);

                await _permissionRepository.DeleteAsync(existingEntity);
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
        /// Associa operaÃ§Ãµes a uma permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> AssignOperationsToPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var result = await AssignOperationsToPermissionInternalAsync(permissionId, operationIds);
                return ResponseBuilder<bool>.Ok(result).Build();
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
        /// Remove operaÃ§Ãµes de uma permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> RemoveOperationsFromPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                //var result = await RemoveOperationsFromPermissionInternalAsync(permissionId, operationIds);
                //return ResponseBuilder<bool>.Ok(result).Build();
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
        /// MÃ©todo interno para associar operaÃ§Ãµes a uma permissÃ£o
        /// </summary>
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
        /// MÃ©todo interno para adicionar ou remover operaÃ§Ãµes de uma permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <param name="permissionOperations"></param>
        /// <returns></returns>
        private async Task<IEnumerable<PermissionOperationEntity>> AddOrRemoveOperationsFromPermissionInternal(Guid permissionId, List<Guid> operationIds,
            IEnumerable<PermissionOperationEntity> permissionOperations)
        {
            try
            {
                foreach (var operationId in operationIds)
                {
                    var permissionOperation = permissionOperations
                        .FirstOrDefault(po => po.OperationId == operationId && po.IsActive);
                    if (permissionOperation != null)
                    {
                        permissionOperation.IsActive = false;
                        permissionOperation.UpdatedAt = DateTime.Now;
                        continue;
                    }
                    permissionOperation = permissionOperations
                        .FirstOrDefault(po => po.OperationId == operationId && !po.IsActive);
                    if (permissionOperation != null)
                    {
                        permissionOperation.IsActive = true;
                        permissionOperation.UpdatedAt = DateTime.Now;
                    }
                }
                return permissionOperations;
            }
            catch (Exception ex)
            {
                return permissionOperations;
            }

        }

        /// <summary>
        /// MÃ©todo interno para remover todas as operaÃ§Ãµes de uma permissÃ£o
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

            await _permissionRepository.DeleteAsync(permission);
            return true;

        }
        private List<Dictionary<Guid, string>> PermissionsOperationsCompare(IEnumerable<PermissionOperationEntity> permissionOperations, PermissionUpdateDTO newPermission)
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
    }
}
