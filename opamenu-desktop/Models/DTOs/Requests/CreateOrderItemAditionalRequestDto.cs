using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.Requests
{
    public class CreateOrderItemAditionalRequestDto
    {
        [JsonPropertyName("aditionalId")]
        public Guid AditionalId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
