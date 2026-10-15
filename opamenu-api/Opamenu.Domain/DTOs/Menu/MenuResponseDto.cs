using Opamenu.Domain.DTOs.Category;
using Opamenu.Domain.DTOs.Coupon;
using Opamenu.Domain.DTOs.Product;
using Opamenu.Domain.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Domain.DTOs.Menu
{
    public class MenuResponseDto
    {
        public TenantBusinessResponseDto? TenantBusiness { get; set; }
        public IEnumerable<ProductDto>? Products { get; set; }
        public IEnumerable<CategoryResponseDto>? Categories { get; set; }
        public IEnumerable<CouponDto>? Coupons { get; set; }
    }
}
