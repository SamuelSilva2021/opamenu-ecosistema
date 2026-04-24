namespace OpaMenu.Domain.DTOs.MultiTenant;

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
