using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Relacionamento entre permissão e operação
    /// Define quais operações uma permissão permite executar
    /// </summary>
    public class PermissionOperationEntity
    {
        /// <summary>
        /// ID do relacionamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID da permissão
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// ID da operação
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Se a operação está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do relacionamento
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        /// <summary>
        /// Permissão relacionada
        /// </summary>
        public virtual PermissionEntity Permission { get; set; } = null!;

        /// <summary>
        /// Operação relacionada
        /// </summary>
        public virtual OperationEntity Operation { get; set; } = null!;
    }
}

