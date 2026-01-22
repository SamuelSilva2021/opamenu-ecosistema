using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;


namespace OpaMenu.Domain.DTOs
{
    public class CreateAddonGroupRequestDto
    {
        /// <summary>
        /// Nome do grupo de adicionais
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Descrição do grupo de adicionais
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }
        /// <summary>
        /// Tipo do grupo de adicionais (Permite apenas uma seleção ou múltiplas seleções)
        /// </summary>
        public EAddonGroupType Type { get; set; } = EAddonGroupType.Multiple;
        /// <summary>
        /// Número mínimo de seleções permitidas
        /// </summary>
        public int? MinSelections { get; set; }
        /// <summary>
        /// Número máximo de seleções permitidas
        /// </summary>
        public int? MaxSelections { get; set; }
        /// <summary>
        /// Indica se o grupo de adicionais é obrigatório
        /// </summary>
        public bool IsRequired { get; set; } = false;
        /// <summary>
        /// Ordem de exibição do grupo de adicionais
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        /// <summary>
        /// Indica se o grupo de adicionais está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}

