using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

public abstract class BaseEntity
{
    /// <summary>
    /// Primary key da entidade.
    /// </summary>
    [Column(name: "id")]
    public Guid Id { get; set; }
    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    [Column(name: "created_at")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Data da ultima atualização da entidade.
    /// </summary>
    [Column(name: "updated_at")]
    public DateTime UpdatedAt { get; set; }
    /// <summary>
    /// Identificador do tenant ao qual a entidade pertence.
    /// </summary>
    [Column(name: "tenant_id")]
    public Guid? TenantId { get; set; }
}

