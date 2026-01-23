using AutoMapper;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class CouponService(
    ICouponRepository couponRepository,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ITenantRepository tenantRepository
    ) : ICouponService
{
    private readonly ICouponRepository _couponRepository = couponRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ITenantRepository _tenantRepository = tenantRepository;

    public async Task<ResponseDTO<IEnumerable<CouponDto>>> GetAllAsync()
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildError("Tenant não identificado.");

            var coupons = await _couponRepository.GetAllAsync(tenantId.Value);
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CouponDto?>> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<CouponDto?>.BuildError("Tenant não identificado.");

            var coupon = await _couponRepository.GetByIdAsync(id, tenantId.Value);
            if (coupon == null)
                return StaticResponseBuilder<CouponDto?>.BuildError("Cupom não encontrado.");

            var dto = _mapper.Map<CouponDto>(coupon);
            return StaticResponseBuilder<CouponDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<CouponDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CouponDto>> CreateAsync(CreateCouponRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<CouponDto>.BuildError("Tenant não identificado.");

            if (!await _couponRepository.IsCodeUniqueAsync(dto.Code, tenantId.Value))
                return StaticResponseBuilder<CouponDto>.BuildError("Já existe um cupom com este código.");

            var coupon = _mapper.Map<CouponEntity>(dto);
            coupon.TenantId = tenantId.Value;
            coupon.CreatedAt = DateTime.UtcNow;
            coupon.UpdatedAt = DateTime.UtcNow;

            await _couponRepository.AddAsync(coupon);
            
            var resultDto = _mapper.Map<CouponDto>(coupon);
            return StaticResponseBuilder<CouponDto>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<CouponDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CouponDto>> UpdateAsync(Guid id, UpdateCouponRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<CouponDto>.BuildError("Tenant não identificado.");

            var coupon = await _couponRepository.GetByIdAsync(id, tenantId.Value);
            if (coupon == null)
                return StaticResponseBuilder<CouponDto>.BuildError("Cupom não encontrado.");

            if (dto.Code != null && dto.Code != coupon.Code)
            {
                if (!await _couponRepository.IsCodeUniqueAsync(dto.Code, tenantId.Value, id))
                    return StaticResponseBuilder<CouponDto>.BuildError("Já existe um cupom com este cÃ³digo.");
            }

            _mapper.Map(dto, coupon);
            coupon.UpdatedAt = DateTime.UtcNow;

            await _couponRepository.UpdateAsync(coupon);

            var resultDto = _mapper.Map<CouponDto>(coupon);
            return StaticResponseBuilder<CouponDto>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<CouponDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<bool>.BuildError("Tenant não identificado.");

            await _couponRepository.DeleteVirtualAsync(id, tenantId.Value);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsForStorefrontAsync()
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildError("Loja não identificada.");

            var coupons = await _couponRepository.GetActiveCouponsAsync(tenantId.Value);
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CouponDto?>> ValidateCouponAsync(string code, decimal orderValue)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<CouponDto?>.BuildError("Loja não identificada.");

            var coupon = await _couponRepository.GetByCodeAsync(code, tenantId.Value);
            return ValidateCouponLogic(coupon, orderValue);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<CouponDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsBySlugAsync(string slug)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
            if (tenantId == Guid.Empty)
                 return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildError("Loja não encontrada.");

            var coupons = await _couponRepository.GetActiveCouponsAsync(tenantId);
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<CouponDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CouponDto?>> ValidateCouponBySlugAsync(string slug, string code, decimal orderValue)
    {
        try
        {
            var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
            if (tenantId == Guid.Empty)
                 return StaticResponseBuilder<CouponDto?>.BuildError("Loja não encontrada.");

            var coupon = await _couponRepository.GetByCodeAsync(code, tenantId);
            return ValidateCouponLogic(coupon, orderValue);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<CouponDto?>.BuildErrorResponse(ex);
        }
    }

    private ResponseDTO<CouponDto?> ValidateCouponLogic(CouponEntity? coupon, decimal orderValue)
    {
        if (coupon == null)
            return StaticResponseBuilder<CouponDto?>.BuildError("Cupom inválido.");

        if (!coupon.IsActive)
            return StaticResponseBuilder<CouponDto?>.BuildError("Cupom inativo.");

        var now = DateTime.UtcNow;
        if (coupon.StartDate.HasValue && coupon.StartDate > now)
            return StaticResponseBuilder<CouponDto?>.BuildError("Cupom ainda não está válido.");

        if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < now)
            return StaticResponseBuilder<CouponDto?>.BuildError("Cupom expirado.");

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit)
            return StaticResponseBuilder<CouponDto?>.BuildError("Limite de uso do cupom atingido.");

        if (coupon.MinOrderValue.HasValue && orderValue < coupon.MinOrderValue)
            return StaticResponseBuilder<CouponDto?>.BuildError($"Valor mánimo do pedido para este cupom: {coupon.MinOrderValue:C}.");

        var dto = _mapper.Map<CouponDto>(coupon);
        return StaticResponseBuilder<CouponDto?>.BuildOk(dto);
    }
}

