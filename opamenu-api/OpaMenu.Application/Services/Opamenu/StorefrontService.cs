using AutoMapper;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.DTOs.Menu;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Application.Services.Opamenu
{
    public class StorefrontService(
        ITenantRepository tenantRepository,
        ITenantBusinessRepository tenantBusinessRepository,
        IMapper mapper,
        IProductRepository productRepository,
        ICouponRepository couponRepository,
        ILoyaltyProgramRepository loyaltyProgramRepository
        ) : IStorefrontService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly ITenantBusinessRepository _tenantBusinessRepository = tenantBusinessRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICouponRepository _couponRepository = couponRepository;
        private readonly ILoyaltyProgramRepository _loyaltyProgramRepository = loyaltyProgramRepository;

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
                {
                    tenantBusinness = _mapper.Map<TenantBusinessResponseDto>(tenantBusinnessEntity);

                    var loyaltyProgram = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
                    if (loyaltyProgram != null && loyaltyProgram.IsActive)
                    {
                        tenantBusinness = tenantBusinness with
                        {
                            LoyaltyProgram = new LoyaltyProgramDto
                            {
                                Id = loyaltyProgram.Id,
                                Name = loyaltyProgram.Name,
                                Description = loyaltyProgram.Description,
                                PointsPerCurrency = loyaltyProgram.PointsPerCurrency,
                                CurrencyValue = loyaltyProgram.CurrencyValue,
                                MinOrderValue = loyaltyProgram.MinOrderValue,
                                PointsValidityDays = loyaltyProgram.PointsValidityDays,
                                IsActive = loyaltyProgram.IsActive
                            }
                        };
                    }
                }

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

