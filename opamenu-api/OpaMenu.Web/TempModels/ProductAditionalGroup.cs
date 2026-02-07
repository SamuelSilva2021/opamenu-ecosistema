using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public partial class ProductAditionalGroup
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int AditionalGroupId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; }

    public int? MinSelectionsOverride { get; set; }

    public int? MaxSelectionsOverride { get; set; }

    public virtual AditionalGroup AditionalGroup { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
