using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Módulo/funcionalidade do sistema
    /// </summary>
    public class ModuleEntity
    {
        /// <summary>
        /// ID único do módulo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do módulo
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do módulo
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL do módulo
        /// </summary>
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Chave única do módulo para identificação
        /// </summary>
        [MaxLength(100)]
        public string? Key { get; set; }

        /// <summary>
        /// Código numérico do módulo
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// ID da aplicação à qual o módulo pertence
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Se o módulo está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ApplicationEntity? Application { get; set; }
    }
}

