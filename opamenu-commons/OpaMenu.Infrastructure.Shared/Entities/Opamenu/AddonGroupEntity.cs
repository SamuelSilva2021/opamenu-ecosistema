using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
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
        /// Descrição opcional do grupo de adicionais.
        /// </summary>
        [MaxLength(200)]
        [Column("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Tipo de grupo de adicionais, que pode ser "Single" (Unica seleção) ou "Multiple" (multiplas seleções).
        /// </summary>
        [Column("type")]
        public EAddonGroupType Type { get; set; } = EAddonGroupType.Multiple;
        /// <summary>
        /// Número mínimo de seleções permitidas para este grupo de adicionais.
        /// </summary>
        [Column("min_selections")]
        public int? MinSelections { get; set; }
        /// <summary>
        /// Número máximo de seleções permitidas para este grupo de adicionais.
        /// </summary>
        [Column("max_selections")]
        public int? MaxSelections { get; set; }
        /// <summary>
        /// Indica se a seleção de pelo menos um adicional deste grupo é obrigatória.
        /// </summary>
        [Column("is_required")]
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// Ordem de exibição do grupo de adicionais na interface do usuário.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;
        /// <summary>
        /// Indica se o grupo de adicionais está ativo e disponível para seleção.
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Lista de adicionais associados a este grupo, como "Banana", "Bacon", etc.
        /// </summary>
        public virtual ICollection<AddonEntity> Addons { get; set; } = new List<AddonEntity>();
        /// <summary>
        /// Lista de associações entre produtos e este grupo de adicionais.
        /// </summary>
        public virtual ICollection<ProductAddonGroupEntity> ProductAddonGroups { get; set; } = new List<ProductAddonGroupEntity>();
    }
}

