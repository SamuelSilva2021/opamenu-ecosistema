using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public partial class Aditional
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int AditionalGroupId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AditionalGroup AditionalGroup { get; set; } = null!;

    public virtual ICollection<OrderItemAditional> OrderItemAditionals { get; set; } = new List<OrderItemAditional>();
}
