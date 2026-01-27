using AutoMapper;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.TenantPaymentConfig;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Opamenu
{
    public class TenantPaymentConfigService(
        ITenantPaymentConfigRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper) : ITenantPaymentConfigService
    {
        public async Task<ResponseDTO<TenantPaymentConfigResponseDto?>> GetConfigAsync()
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (tenantId == null) return StaticResponseBuilder<TenantPaymentConfigResponseDto?>.BuildError("Tenant não identificado.");

                // Get active PIX config
                var entity = await repository.GetActivePixConfigAsync(tenantId.Value);
                
                if (entity == null)
                {
                    return StaticResponseBuilder<TenantPaymentConfigResponseDto?>.BuildOk(null);
                }

                var dto = mapper.Map<TenantPaymentConfigResponseDto>(entity);
                return StaticResponseBuilder<TenantPaymentConfigResponseDto?>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<TenantPaymentConfigResponseDto?>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<TenantPaymentConfigResponseDto>> UpsertConfigAsync(UpsertTenantPaymentConfigRequestDto dto)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid();
                if (tenantId == null) return StaticResponseBuilder<TenantPaymentConfigResponseDto>.BuildError("Tenant não identificado.");

                // 1. If activating, deactivate others of the same PaymentMethod
                if (dto.IsActive)
                {
                    var allConfigs = await repository.GetActiveConfigsAsync(tenantId.Value);
                    var others = allConfigs.Where(x => x.PaymentMethod == dto.PaymentMethod && x.Provider != dto.Provider).ToList();
                    foreach (var other in others)
                    {
                        other.IsActive = false;
                        await repository.UpdateAsync(other);
                    }
                }

                // 2. Find specific config to update or create
                // We don't have GetByProvider in repo, but we can use generic FirstOrDefaultAsync since we added IRepository inheritance
                var existingEntity = await repository.FirstOrDefaultAsync(x => 
                    x.TenantId == tenantId.Value && 
                    x.PaymentMethod == dto.PaymentMethod && 
                    x.Provider == dto.Provider);

                TenantPaymentConfigEntity entityToReturn;

                if (existingEntity != null)
                {
                    mapper.Map(dto, existingEntity);
                    await repository.UpdateAsync(existingEntity);
                    entityToReturn = existingEntity;
                }
                else
                {
                    var newEntity = mapper.Map<TenantPaymentConfigEntity>(dto);
                    newEntity.TenantId = tenantId.Value;
                    await repository.AddAsync(newEntity);
                    entityToReturn = newEntity;
                }

                var responseDto = mapper.Map<TenantPaymentConfigResponseDto>(entityToReturn);
                return StaticResponseBuilder<TenantPaymentConfigResponseDto>.BuildOk(responseDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<TenantPaymentConfigResponseDto>.BuildErrorResponse(ex);
            }
        }
    }
}
