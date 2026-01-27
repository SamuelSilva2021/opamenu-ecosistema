using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl;

/// <summary>
/// Representa um usuário do sistema
/// </summary>
[Table("users")]
public class User : BaseEntity
{
    /// <summary>
    /// Email único do usuário
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha do usuário
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Primeiro nome do usuÃ¡rio
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Ãšltimo nome do usuÃ¡rio
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do usuÃ¡rio
    /// </summary>
    [MaxLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// URL do avatar do usuÃ¡rio
    /// </summary>
    [MaxLength(500)]
    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Status do usuÃ¡rio (active, inactive, suspended)
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active";

    /// <summary>
    /// Data de verificaÃ§Ã£o do email
    /// </summary>
    [Column("email_verified_at")]
    public DateTime? EmailVerifiedAt { get; set; }

    /// <summary>
    /// Data do Ãºltimo login
    /// </summary>
    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// ConfiguraÃ§Ãµes personalizadas do usuÃ¡rio
    /// </summary>
    [Column("preferences")]
    public string Preferences { get; set; } = "{}";

    /// <summary>
    /// Nome completo do usuÃ¡rio
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Tenant ao qual o usuÃ¡rio pertence
    /// </summary>
    public virtual TenantEntity? Tenant { get; set; }

    /// <summary>
    /// Indica se o usuÃ¡rio Ã© administrador da plataforma
    /// </summary>
    public bool IsPlatformAdmin => TenantId == null;
}
