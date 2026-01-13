using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Menu
{
    public class MenuResponseDto
    {
        public TenantBusinessResponseDto? TenantBusiness { get; set; }
        public IEnumerable<ProductDto>? Products { get; set; }
        public IEnumerable<CategoryResponseDto>? Categories { get; set; }
        public IEnumerable<CouponDto>? Coupons { get; set; }
    }
}
