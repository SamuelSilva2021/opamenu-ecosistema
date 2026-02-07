using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public partial class AditionalGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Type { get; set; }

    public int? MinSelections { get; set; }

    public int? MaxSelections { get; set; }

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Aditional> Aditionals { get; set; } = new List<Aditional>();

    public virtual ICollection<ProductAditionalGroup> ProductAditionalGroups { get; set; } = new List<ProductAditionalGroup>();
}
