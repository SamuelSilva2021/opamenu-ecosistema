using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    /// <summary>
    /// Representa a associação entre um produto e um grupo de adicionais.
    /// </summary>
    [Table("product_aditional_groups")]
    public class ProductAditionalGroupEntity : BaseEntity
    {
        /// <summary>
        /// Identificador do produto associado a este grupo de adicionais.
        /// </summary>
        [Column("product_id")]
        public Guid ProductId { get; set; }
        /// <summary>
        /// Identificador do grupo de adicionais associado a este produto.
        /// </summary>
        [Column("aditional_group_id")]
        public Guid AditionalGroupId { get; set; }
        /// <summary>
        /// Ordem de exibição do grupo de adicionais para este produto.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;
        /// <summary>
        /// Indica se o grupo de adicionais é obrigatório para este produto.
        /// </summary>
        [Column("is_required")]
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// Número mínimo de seleções permitidas para este grupo de adicionais.
        /// </summary>
        [Column("min_selections_override")]
        public int? MinSelectionsOverride { get; set; }
        /// <summary>
        /// Número máximo de seleções permitidas para este grupo de adicionais.
        /// </summary>
        [Column("max_selections_override")]
        public int? MaxSelectionsOverride { get; set; }
        /// <summary>
        /// Representa o produto associado a este grupo de adicionais.
        /// </summary>
        public virtual ProductEntity Product { get; set; } = null!;
        /// <summary>
        /// Representa o grupo de adicionais associado a este produto.
        /// </summary>
        public virtual AditionalGroupEntity AditionalGroup { get; set; } = null!;
    }
}
