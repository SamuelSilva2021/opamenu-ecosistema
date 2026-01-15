using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Application.Services.Interfaces;

public interface ICouponService
{
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetAllAsync();
    Task<ResponseDTO<CouponDto?>> GetByIdAsync(int id);
    Task<ResponseDTO<CouponDto>> CreateAsync(CreateCouponRequestDto dto);
    Task<ResponseDTO<CouponDto>> UpdateAsync(int id, UpdateCouponRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(int id);
    
    // Storefront methods
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsForStorefrontAsync();
    Task<ResponseDTO<CouponDto?>> ValidateCouponAsync(string code, decimal orderValue);

    // Public Menu (Slug-based) methods
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsBySlugAsync(string slug);
    Task<ResponseDTO<CouponDto?>> ValidateCouponBySlugAsync(string slug, string code, decimal orderValue);
}
