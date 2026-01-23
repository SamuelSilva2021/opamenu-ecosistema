using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant
{
    /// <summary>
    /// Entidade que representa os detalhes bancários de um cliente (Tenant)
    /// </summary>
    [Table("bank_details")]
    public class BankDetailsEntity 
    {
        /// <summary>
        /// Primary key da entidade.
        /// </summary>
        [Column(name: "id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Identificador do tenant ao qual a entidade pertence.
        /// </summary>
        [Column(name: "tenant_id")]
        public Guid TenantId { get; set; }
        /// <summary>
        /// Nome do banco
        /// </summary>
        [Column("bank_name")]
        public string? BankName { get; set; }
        /// <summary>
        /// Agência bancária
        /// </summary>
        [Column("agency")]
        public string? Agency { get; set; }
        /// <summary>
        /// Conta bancária
        /// </summary>
        [Column("account_number")]
        public string? AccountNumber { get; set; }
        /// <summary>
        /// Tipo da conta (corrente, poupança, digital)
        /// </summary>
        [Column("account_type")]
        public int? AccountType { get; set; }
        /// <summary>
        /// Código do banco
        /// </summary>
        [Column("bank_id")]
        public int? BankId { get; set; }
        /// <summary>
        /// Chave pix da conta
        /// </summary>
        [Column("pix_key")]
        public string? PixKey { get; set; }
        [Column("pix_key_selected")]
        /// <summary>
        /// Indica se a chave pix foi selecionada.
        /// </summary>
        public bool IsPixKeySelected { get; set; } = false;
        /// <summary>
        /// Indica se o registro está ativo.
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Data de criação da entidade.
        /// </summary>
        [Column(name: "created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Data da ultima atualização da entidade.
        /// </summary>
        [Column(name: "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public virtual TenantEntity Tenant { get; set; } = null!;
    }
}
