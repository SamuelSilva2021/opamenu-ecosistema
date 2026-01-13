using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Tipo de papel/função no sistema
    /// </summary>
    public class RoleTypeEntity
    {
        /// <summary>
        /// ID único do tipo de papel
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nome do tipo de papel ex: Administrador, Usuário
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;
        /// <summary>
        /// Descrição do tipo de papel ex: Papel com permissões administrativas, Papel com permissões de usuário comum
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Lista de papéis associados a este tipo
        /// </summary>
        public ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    }
}
