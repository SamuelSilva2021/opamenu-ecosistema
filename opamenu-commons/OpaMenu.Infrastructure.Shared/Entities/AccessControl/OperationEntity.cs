using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{

    /// <summary>
    /// Operações que podem ser realizadas no sistema
    /// </summary>
    public class OperationEntity
    {
        /// <summary>
        /// ID da operação
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da operação
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da operação
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Valor da operação (ex: 'CREATE', 'READ', 'UPDATE','DELETE')
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Se a operação está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Relações com permissões
        /// </summary>
        public virtual ICollection<PermissionOperationEntity> PermissionOperations { get; set; } = new List<PermissionOperationEntity>();
    }
}

