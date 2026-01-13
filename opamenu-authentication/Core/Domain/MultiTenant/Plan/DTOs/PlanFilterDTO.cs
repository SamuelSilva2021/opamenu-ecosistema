using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanFilterDTO
    {
        public string? Name { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? BillingCycle { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinUsers { get; set; }
        public int? MaxUsers { get; set; }
        public int? MinStorage { get; set; }
        public int? MaxStorage { get; set; }
        public EPlanStatus Status { get; set; }
        public bool? IsTrial { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "sortorder";
        public bool SortDescending { get; set; } = false;
    }
}

