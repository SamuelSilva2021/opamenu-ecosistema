using AutoMapper;
using OpaMenu.Application.Common.Builders;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.TenantPaymentMethod;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services;

public class TenantPaymentMethodService(
    ITenantPaymentMethodRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper) : ITenantPaymentMethodService
{
    public async Task<ResponseDTO<IEnumerable<TenantPaymentMethodResponseDto>>> GetAllByTenantAsync()
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<IEnumerable<TenantPaymentMethodResponseDto>>.BuildError("Tenant nÃ£o identificado.");

            var entities = await repository.GetAllByTenantWithPaymentMethodAsync(tenantId.Value);
            var dtos = mapper.Map<IEnumerable<TenantPaymentMethodResponseDto>>(entities);
            return StaticResponseBuilder<IEnumerable<TenantPaymentMethodResponseDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<TenantPaymentMethodResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TenantPaymentMethodResponseDto?>> GetByIdAsync(int id)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TenantPaymentMethodResponseDto?>.BuildError("Tenant nÃ£o identificado.");

            var entity = await repository.GetByIdWithPaymentMethodAsync(id, tenantId.Value);
            if (entity == null) return StaticResponseBuilder<TenantPaymentMethodResponseDto?>.BuildNotFound(null);

            var dto = mapper.Map<TenantPaymentMethodResponseDto>(entity);
            return StaticResponseBuilder<TenantPaymentMethodResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TenantPaymentMethodResponseDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TenantPaymentMethodResponseDto>> CreateAsync(CreateTenantPaymentMethodRequestDto dto)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildError("Tenant nÃ£o identificado.");

            var entity = mapper.Map<TenantPaymentMethodEntity>(dto);
            entity.TenantId = tenantId.Value;
            
            // Define display order as last
            var existing = await repository.GetAllAsync(tenantId.Value);
            entity.DisplayOrder = existing.Any() ? existing.Max(x => x.DisplayOrder) + 1 : 1;

            await repository.AddAsync(entity);
            
            // Reload with includes for response
            var createdEntity = await repository.GetByIdWithPaymentMethodAsync(entity.Id, tenantId.Value);
            var responseDto = mapper.Map<TenantPaymentMethodResponseDto>(createdEntity);
            
            return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildOk(responseDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TenantPaymentMethodResponseDto>> UpdateAsync(int id, UpdateTenantPaymentMethodRequestDto dto)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildError("Tenant nÃ£o identificado.");

            var entity = await repository.GetByIdAsync(id, tenantId.Value);
            if (entity == null) return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildNotFound(null);

            mapper.Map(dto, entity);
            await repository.UpdateAsync(entity);

            // Reload with includes
            var updatedEntity = await repository.GetByIdWithPaymentMethodAsync(id, tenantId.Value);
            var responseDto = mapper.Map<TenantPaymentMethodResponseDto>(updatedEntity);

            return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildOk(responseDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TenantPaymentMethodResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(int id)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<bool>.BuildError("Tenant nÃ£o identificado.");

            await repository.DeleteVirtualAsync(id, tenantId.Value);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> ToggleActiveAsync(int id)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<bool>.BuildError("Tenant nÃ£o identificado.");

            var entity = await repository.GetByIdAsync(id, tenantId.Value);
            if (entity == null) return StaticResponseBuilder<bool>.BuildNotFound(false);

            entity.IsActive = !entity.IsActive;
            await repository.UpdateAsync(entity);

            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }
    
    public async Task<ResponseDTO<bool>> ReorderAsync(List<int> orderedIds)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<bool>.BuildError("Tenant nÃ£o identificado.");
            
            var entities = await repository.GetAllAsync(tenantId.Value);
            var entityMap = entities.ToDictionary(x => x.Id);
            
            for (int i = 0; i < orderedIds.Count; i++)
            {
                if (entityMap.TryGetValue(orderedIds[i], out var entity))
                {
                    entity.DisplayOrder = i + 1;
                    await repository.UpdateAsync(entity);
                }
            }
            
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
             return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }
}

