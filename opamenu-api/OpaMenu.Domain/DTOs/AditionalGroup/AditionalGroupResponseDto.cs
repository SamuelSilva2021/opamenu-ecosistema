using OpaMenu.Domain.DTOs.Aditionals;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.DTOs.AditionalGroup
{
    /// <summary>
    /// DTO de resposta para grupo de adicionais
    /// </summary>
    public class AditionalGroupResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EAditionalGroupType Type { get; set; }
        public int? MinSelections { get; set; }
        public int? MaxSelections { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<AditionalResponseDto> Aditionals { get; set; } = new();
    }
}
