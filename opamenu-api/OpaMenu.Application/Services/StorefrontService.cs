using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Domain.DTOs.Menu;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Application.Services
{
    public class StorefrontService(
        ITenantRepository tenantRepository,
        ITenantBusinessRepository tenantBusinessRepository,
        IMapper mapper,
        IProductRepository productRepository,
        ICouponRepository couponRepository
        ) : IStorefrontService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly ITenantBusinessRepository _tenantBusinessRepository = tenantBusinessRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICouponRepository _couponRepository = couponRepository;

        public async Task<ResponseDTO<MenuResponseDto>> GetStorefrontDataAsync(string slug)
        {
            try
            {
                var tenantId = await _tenantRepository.GetTenantIdBySlugAsyn(slug);
                TenantBusinessResponseDto? tenantBusinness = null;
                IEnumerable<ProductDto> products = [];
                IEnumerable<CategoryResponseDto>? categories = null;
                IEnumerable<CouponDto> coupons = [];

                var tenantBusinnessEntity = await _tenantBusinessRepository.GetByTenantIdAsync(tenantId); 
                    
                if (tenantBusinnessEntity == null)
                    return StaticResponseBuilder<MenuResponseDto>.BuildError("Loja nÃ£o encontrada.");

                if (tenantBusinnessEntity != null)
                    tenantBusinness = _mapper.Map<TenantBusinessResponseDto>(tenantBusinnessEntity);

                var productsEntity = await _productRepository.GetProductsForMenuAsync(tenantId);
                if (productsEntity.Any())
                    products = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);

                var cateriesEntity = productsEntity.Select(p => p.Category).Distinct();
                if (cateriesEntity.Any())
                    categories = _mapper.Map<IEnumerable<CategoryResponseDto>>(cateriesEntity);

                var couponsEntity = await _couponRepository.GetActiveCouponsAsync(tenantId);

                if (couponsEntity.Any())
                    coupons = _mapper.Map<IEnumerable<CouponDto>>(couponsEntity);

                var menu = new MenuResponseDto
                {
                    TenantBusiness = tenantBusinness,
                    Products = products,
                    Categories = categories,
                    Coupons = coupons
                };

                return StaticResponseBuilder<MenuResponseDto>.BuildOk(menu);

            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<MenuResponseDto>.BuildErrorResponse(ex);
            }
        }
    }
}

