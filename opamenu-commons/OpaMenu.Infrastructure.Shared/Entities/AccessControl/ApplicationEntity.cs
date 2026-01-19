using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Aplicação/sistema que utiliza o serviço de autenticação
    /// </summary>
    public class ApplicationEntity
    {
        /// <summary>
        /// ID único da aplicação
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da aplicação
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da aplicação
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Chave secreta da aplicação para autenticação
        /// </summary>
        [MaxLength(255)]
        public string? SecretKey { get; set; }

        /// <summary>
        /// URL da aplicação
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Código numérico da aplicação
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Schema auxiliar da aplicação
        /// </summary>
        public string? AuxiliarSchema { get; set; }

        /// <summary>
        /// Se a aplicação está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Se a aplicação está visível
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ModuleEntity> Modules { get; set; } = new List<ModuleEntity>();
    }
}

