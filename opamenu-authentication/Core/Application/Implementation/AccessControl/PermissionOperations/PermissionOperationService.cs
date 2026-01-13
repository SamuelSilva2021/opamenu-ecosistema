using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.PermissionOperations
{
    /// <summary>
    /// ServiÃ§o para gerenciar relaÃ§Ãµes PermissÃ£o-OperaÃ§Ã£o
    /// </summary>
    /// <param name="permissionOperationRepository"></param>
    /// <param name="permissionRepository"></param>
    /// <param name="operationRepository"></param>
    /// <param name="mapper"></param>
    public class PermissionOperationService(
        IPermissionOperationRepository permissionOperationRepository,
        IPermissionRepository permissionRepository,
        IOperationRepository operationRepository,
        IMapper mapper) : IPermissionOperationService
    {
        private readonly IPermissionOperationRepository _permissionOperationRepository = permissionOperationRepository;
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// ObtÃ©m todas as relaÃ§Ãµes PermissÃ£o-OperaÃ§Ã£o
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetAllPermissionOperationsAsync()
        {
            try
            {
                var entities = await _permissionOperationRepository.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m relaÃ§Ãµes por ID da permissÃ£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var entities = await _permissionOperationRepository.GetByPermissionIdAsync(permissionId);
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m relaÃ§Ãµes por ID da operaÃ§Ã£o
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByOperationIdAsync(Guid operationId)
        {
            try
            {
                var entities = await _permissionOperationRepository.GetByOperationIdAsync(operationId);
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// ObtÃ©m uma relaÃ§Ã£o especÃ­fica entre permissÃ£o e operaÃ§Ã£o
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId)
        {
            try
            {
                var entity = await _permissionOperationRepository.GetByPermissionAndOperationAsync(permissionId, operationId);
                
                if (entity == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "RelaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                var dto = _mapper.Map<PermissionOperationDTO>(entity);
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Cria uma nova relaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o
        /// </summary>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> CreatePermissionOperationAsync(PermissionOperationCreateDTO permissionOperation)
        {
            try
            {
                // Validar se a permissÃ£o existe
                var permission = await _permissionRepository.GetByIdAsync(permissionOperation.PermissionId);
                if (permission == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "PermissÃ£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                // Validar se a operaÃ§Ã£o existe
                var operation = await _operationRepository.GetByIdAsync(permissionOperation.OperationId);
                if (operation == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "OperaÃ§Ã£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                // Verificar se a relaÃ§Ã£o jÃ¡ existe
                var existingRelation = await _permissionOperationRepository.GetByPermissionAndOperationAsync(
                    permissionOperation.PermissionId, permissionOperation.OperationId);
                
                if (existingRelation != null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "RelaÃ§Ã£o jÃ¡ existe entre a permissÃ£o e operaÃ§Ã£o" })
                        .WithCode(409)
                        .Build();

                var entity = _mapper.Map<PermissionOperationEntity>(permissionOperation);
                entity.CreatedAt = DateTime.Now;

                var createdEntity = await _permissionOperationRepository.AddAsync(entity);
                var dto = _mapper.Map<PermissionOperationDTO>(createdEntity);
                
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Cria mÃºltiplas relaÃ§Ãµes PermissÃ£o-OperaÃ§Ã£o
        /// </summary>
        /// <param name="permissionOperations"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> CreatePermissionOperationsBulkAsync(PermissionOperationBulkDTO permissionOperations)
        {
            try
            {
                var results = new List<PermissionOperationDTO>();

                // Validar se a permissÃ£o existe
                var permission = await _permissionRepository.GetByIdAsync(permissionOperations.PermissionId);
                if (permission == null)
                    return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                        .Fail(new ErrorDTO { Message = "PermissÃ£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                foreach (var operationId in permissionOperations.OperationIds)
                {
                    // Validar se a operaÃ§Ã£o existe
                    var operation = await _operationRepository.GetByIdAsync(operationId);
                    if (operation == null)
                        continue; // Pular operaÃ§Ãµes que nÃ£o existem

                    // Verificar se a relaÃ§Ã£o jÃ¡ existe
                    var existingRelation = await _permissionOperationRepository.GetByPermissionAndOperationAsync(
                        permissionOperations.PermissionId, operationId);
                    
                    if (existingRelation != null)
                        continue; // Pular relaÃ§Ãµes que jÃ¡ existem

                    var entity = new PermissionOperationEntity
                    {
                        Id = Guid.NewGuid(),
                        PermissionId = permissionOperations.PermissionId,
                        OperationId = operationId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    var createdEntity = await _permissionOperationRepository.AddAsync(entity);
                    var dto = _mapper.Map<PermissionOperationDTO>(createdEntity);
                    results.Add(dto);
                }

                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(results).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Atualiza uma relaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> UpdatePermissionOperationAsync(Guid id, PermissionOperationUpdateDTO permissionOperation)
        {
            try
            {
                var existingEntity = await _permissionOperationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "RelaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                _mapper.Map(permissionOperation, existingEntity);
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionOperationRepository.UpdateAsync(existingEntity);
                var dto = _mapper.Map<PermissionOperationDTO>(existingEntity);
                
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Remove uma relaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeletePermissionOperationAsync(Guid id)
        {
            try
            {
                var existingEntity = await _permissionOperationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "RelaÃ§Ã£o PermissÃ£o-OperaÃ§Ã£o nÃ£o encontrada" })
                        .WithCode(404)
                        .Build();

                existingEntity.IsActive = false;
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionOperationRepository.UpdateAsync(existingEntity);
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
        /// Remove todas as relaÃ§Ãµes de uma permissÃ£o (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteAllByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var result = await _permissionOperationRepository.RemoveAllByPermissionIdAsync(permissionId);
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
        /// Remove relaÃ§Ãµes especÃ­ficas de uma permissÃ£o (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds)
        {
            try
            {
                var result = await _permissionOperationRepository.RemoveByPermissionAndOperationsAsync(permissionId, operationIds);
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
    }
}
