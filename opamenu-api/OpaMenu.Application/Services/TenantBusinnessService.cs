using AutoMapper;
using OpaMenu.Application.Common.Builders;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System.Text.Json;

namespace OpaMenu.Application.Services;

public class TenantBusinnessService(
    ITenantRepository tenantRepository,
    ITenantBusinessRepository tenantBusinessRepository,
    IMapper mapper,
    ICurrentUserService currentUserService) : ITenantBusinnessService
{
    public async Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoByTenantId()
    {
        try
        {
            var tenant = await tenantRepository.GetByIdAsync(currentUserService.GetTenantGuid()!.Value);

            if (tenant == null)
                return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildNotFound(null);

            var dto = mapper.Map<TenantBusinessResponseDto>(tenant);
            return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TenantBusinessResponseDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TenantBusinessResponseDto>> UpdateTenantBusinessInfo(UpdateTenantBusinessRequestDto dto)
    {
        try
        {
            var tenantId = currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TenantBusinessResponseDto>.BuildError("Tenant nÃ£o identificado.");

            var tenant = await tenantRepository.GetByIdAsync(tenantId.Value);
            if (tenant == null) return StaticResponseBuilder<TenantBusinessResponseDto>.BuildNotFound(null);

            if (dto.Name != null) tenant.Name = dto.Name;
            if (dto.Phone != null) tenant.Phone = dto.Phone;
            if (dto.Email != null) tenant.Email = dto.Email;
            if (dto.AddressStreet != null) tenant.AddressStreet = dto.AddressStreet;
            if (dto.AddressNumber != null) tenant.AddressNumber = dto.AddressNumber;
            if (dto.AddressComplement != null) tenant.AddressComplement = dto.AddressComplement;
            if (dto.AddressNeighborhood != null) tenant.AddressNeighborhood = dto.AddressNeighborhood;
            if (dto.AddressCity != null) tenant.AddressCity = dto.AddressCity;
            if (dto.AddressState != null) tenant.AddressState = dto.AddressState;
            if (dto.AddressZipcode != null) tenant.AddressZipcode = dto.AddressZipcode;

            if (tenant.BusinessInfo == null)
            {
                tenant.BusinessInfo = new TenantBusinessEntity
                {
                    TenantId = tenant.Id,
                    LogoUrl = dto.LogoUrl,
                    BannerUrl = dto.BannerUrl,
                    Description = dto.Description,
                    InstagramUrl = dto.InstagramUrl,
                    FacebookUrl = dto.FacebookUrl,
                    WhatsappNumber = dto.WhatsappNumber,
                    OpeningHours = Serialize(dto.OpeningHours),
                    PaymentMethods = Serialize(dto.PaymentMethods),
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude
                };
                await tenantBusinessRepository.AddAsync(tenant.BusinessInfo);
            }
            else
            {
                var info = tenant.BusinessInfo;
                if (dto.LogoUrl != null) info.LogoUrl = dto.LogoUrl;
                if (dto.BannerUrl != null) info.BannerUrl = dto.BannerUrl;
                if (dto.Description != null) info.Description = dto.Description;
                if (dto.InstagramUrl != null) info.InstagramUrl = dto.InstagramUrl;
                if (dto.FacebookUrl != null) info.FacebookUrl = dto.FacebookUrl;
                if (dto.WhatsappNumber != null) info.WhatsappNumber = dto.WhatsappNumber;
                if (dto.OpeningHours != null) info.OpeningHours = Serialize(dto.OpeningHours);
                if (dto.PaymentMethods != null) info.PaymentMethods = Serialize(dto.PaymentMethods);
                if (dto.Latitude.HasValue) info.Latitude = dto.Latitude;
                if (dto.Longitude.HasValue) info.Longitude = dto.Longitude;

                await tenantBusinessRepository.UpdateAsync(info);
            }

            await tenantRepository.UpdateAsync(tenant);

            var updatedTenant = await tenantRepository.GetByIdAsync(tenantId.Value);
            var responseDto = mapper.Map<TenantBusinessResponseDto>(updatedTenant);

            return StaticResponseBuilder<TenantBusinessResponseDto>.BuildOk(responseDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TenantBusinessResponseDto>.BuildErrorResponse(ex);
        }
    }

    private static string? Serialize(object? obj)
    {
        if (obj == null) return null;
        if (obj is JsonElement jsonElement) return jsonElement.ToString();
        if (obj is string str) return str;
        return JsonSerializer.Serialize(obj);
    }
}

