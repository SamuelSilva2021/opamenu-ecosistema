namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}
