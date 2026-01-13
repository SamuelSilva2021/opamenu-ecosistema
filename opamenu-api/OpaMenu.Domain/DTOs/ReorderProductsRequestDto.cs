using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs
{
    public class ReorderProductsRequestDto
    {

        [Required]
        public Dictionary<int, int> ProductOrders { get; set; } = new();
    }
}
