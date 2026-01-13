using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OpaMenu.Infrastructure.Shared.Entities
{
    /// <summary>
    /// Representa a associaÃ§Ã£o entre um produto e um grupo de addons.
    /// </summary>
    [Table("product_addon_groups")]
    public class ProductAddonGroupEntity : BaseEntity
    {
        /// <summary>
        /// Identificador do produto associado a este grupo de adicionais.
        /// </summary>
        [Column("product_id")]
        public int ProductId { get; set; }
        /// <summary>
        /// Identificador do grupo de adicionais associado a este produto.
        /// </summary>
        [Column("addon_group_id")]
        public int AddonGroupId { get; set; }
        /// <summary>
        /// Ordem de exibiÃ§Ã£o do grupo de adicionais para este produto.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;
        /// <summary>
        /// Indica se o grupo de adicionais Ã© obrigatÃ³rio para este produto.
        /// </summary>
        [Column("is_required")]
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// NÃºmero mÃ­nimo de seleÃ§Ãµes permitidas para este grupo de adicionais.
        /// </summary>
        [Column("min_selections_override")]
        public int? MinSelectionsOverride { get; set; }
        /// <summary>
        /// NÃºmero mÃ¡ximo de seleÃ§Ãµes permitidas para este grupo de adicionais.
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
        public virtual AddonGroupEntity AddonGroup { get; set; } = null!;
    }
}

