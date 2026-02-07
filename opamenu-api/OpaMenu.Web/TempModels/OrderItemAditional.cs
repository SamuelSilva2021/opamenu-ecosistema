using OpaMenu.Infrastructure.Shared.Entities;
using System;
using System.Collections.Generic;

namespace OpaMenu.Web.TempModels;

public partial class OrderItemAditional
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public int AditionalId { get; set; }

    public string AditionalName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Aditional Aditional { get; set; } = null!;

    public virtual OrderItem OrderItem { get; set; } = null!;
}
