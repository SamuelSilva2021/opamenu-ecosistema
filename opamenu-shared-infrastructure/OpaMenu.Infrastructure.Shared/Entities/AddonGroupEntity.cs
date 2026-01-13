using OpaMenu.Infrastructure.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    [Table("addon_groups")]
    public class AddonGroupEntity : BaseEntity
    {
        /// <summary>
        /// Nome do grupo de adicionais, como "Frutas", "Coberturas", "Extras".
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty; // Ex: "Frutas", "Coberturas", "Extras"
        /// <summary>
        /// DescriÃ§Ã£o opcional do grupo de adicionais, como "Escolha suas frutas favoritas".
        /// </summary>
        [MaxLength(200)]
        [Column("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Tipo de grupo de adicionais, que pode ser "Single" (Ãºnica seleÃ§Ã£o) ou "Multiple" (mÃºltiplas seleÃ§Ãµes).
        /// </summary>
        [Column("type")]
        public EAddonGroupType Type { get; set; } = EAddonGroupType.Multiple;
        /// <summary>
        /// NÃºmero mÃ­nimo de seleÃ§Ãµes permitidas para este grupo de adicionais.
        /// </summary>
        [Column("min_selections")]
        public int? MinSelections { get; set; }
        /// <summary>
        /// NÃºmero mÃ¡ximo de seleÃ§Ãµes permitidas para este grupo de adicionais.
        /// </summary>
        [Column("max_selections")]
        public int? MaxSelections { get; set; }
        /// <summary>
        /// Indica se este grupo de adicionais Ã© obrigatÃ³rio para os produtos associados.
        /// </summary>
        [Column("is_required")]
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// Ordem de exibiÃ§Ã£o do grupo de adicionais na interface do usuÃ¡rio.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;
        /// <summary>
        /// Indica se o grupo de adicionais estÃ¡ ativo e disponÃ­vel para seleÃ§Ã£o.
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Lista de adicionais associados a este grupo, como "Banana", "Bacon", etc.
        /// </summary>
        public virtual ICollection<AddonEntity> Addons { get; set; } = new List<AddonEntity>();
        /// <summary>
        /// Lista de associaÃ§Ãµes entre produtos e este grupo de adicionais, permitindo que o grupo seja usado em diferentes produtos.
        /// </summary>
        public virtual ICollection<ProductAddonGroupEntity> ProductAddonGroups { get; set; } = new List<ProductAddonGroupEntity>();
    }
}

