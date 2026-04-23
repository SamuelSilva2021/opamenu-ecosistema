namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class PlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}

public sealed class CreatePlanRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}

public sealed class UpdatePlanRequestDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? BillingCycle { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxStorageGb { get; set; }
    public string? Features { get; set; }
    public string? Status { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsTrial { get; set; }
    public int? TrialPeriodDays { get; set; }
}

public sealed class PlanListResponseDto
{
    public List<PlanDto> Items { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public bool Succeeded { get; set; }
    public int Code { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public sealed class ApiResponseDto<T>
{
    public bool Succeeded { get; set; }
    public T Data { get; set; } = default!;
    public List<ErrorDto> Errors { get; set; } = [];
}

public sealed class ErrorDto
{
    public string Message { get; set; } = string.Empty;
}

