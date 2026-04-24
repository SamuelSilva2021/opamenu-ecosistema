namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class RegisterTenantResponseDto
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = "Tenant cadastrado com sucesso!";
    public bool RedirectToPlanSelection { get; set; }
}
