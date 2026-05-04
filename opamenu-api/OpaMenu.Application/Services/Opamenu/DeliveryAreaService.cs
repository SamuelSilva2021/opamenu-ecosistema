using AutoMapper;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.DeliveryArea;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class DeliveryAreaService(
    IDeliveryAreaRepository repository,
    ITenantRepository tenantRepository,
    IMapper mapper) : IDeliveryAreaService
{
    public async Task<ResponseDTO<IEnumerable<DeliveryAreaResponseDto>>> GetAllAsync(Guid tenantId)
    {
        try
        {
            var entities = await repository.GetAllByTenantAsync(tenantId);
            var result = mapper.Map<IEnumerable<DeliveryAreaResponseDto>>(entities);
            return StaticResponseBuilder<IEnumerable<DeliveryAreaResponseDto>>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<DeliveryAreaResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<DeliveryAreaResponseDto?>> GetByIdAsync(Guid id, Guid tenantId)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, tenantId);
            var result = mapper.Map<DeliveryAreaResponseDto?>(entity);
            return StaticResponseBuilder<DeliveryAreaResponseDto?>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<DeliveryAreaResponseDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<DeliveryAreaResponseDto>> CreateAsync(CreateDeliveryAreaRequestDto request, Guid tenantId)
    {
        try
        {
            var entity = mapper.Map<DeliveryAreaEntity>(request);
            entity.TenantId = tenantId;
            
            await repository.AddAsync(entity);
            var result = mapper.Map<DeliveryAreaResponseDto>(entity);
            return StaticResponseBuilder<DeliveryAreaResponseDto>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<DeliveryAreaResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<DeliveryAreaResponseDto>> UpdateAsync(Guid id, CreateDeliveryAreaRequestDto request, Guid tenantId)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, tenantId);
            if (entity == null) throw new KeyNotFoundException("Regra de entrega não encontrada.");

            mapper.Map(request, entity);
            await repository.UpdateAsync(entity);
            
            var result = mapper.Map<DeliveryAreaResponseDto>(entity);
            return StaticResponseBuilder<DeliveryAreaResponseDto>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<DeliveryAreaResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id, Guid tenantId)
    {
        try
        {
            await repository.DeleteVirtualAsync(id, tenantId);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<decimal?>> GetDeliveryFeeAsync(string slug, string city, string? neighborhood)
    {
        try
        {
            var tenantId = await tenantRepository.GetTenantIdBySlugAsyn(slug);
            if (tenantId == Guid.Empty) return StaticResponseBuilder<decimal?>.BuildError("Estabelecimento não encontrado.");

            var rule = await repository.GetByLocationAsync(tenantId, city, neighborhood);
            return StaticResponseBuilder<decimal?>.BuildOk(rule?.Fee);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<decimal?>.BuildErrorResponse(ex);
        }
    }
}
