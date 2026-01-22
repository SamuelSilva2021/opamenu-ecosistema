using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    public class UpdateAddonGroupRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public EAddonGroupType Type { get; set; }

        public int? MinSelections { get; set; }

        public int? MaxSelections { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}

