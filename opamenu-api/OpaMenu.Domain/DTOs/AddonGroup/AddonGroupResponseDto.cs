using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.AddonGroup
{
    /// <summary>
    /// DTO de resposta para grupo de adicionais
    /// </summary>
    public class AddonGroupResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EAddonGroupType Type { get; set; }
        public int? MinSelections { get; set; }
        public int? MaxSelections { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<AddonResponseDto> Addons { get; set; } = new();
    }
}

