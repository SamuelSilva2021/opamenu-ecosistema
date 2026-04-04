using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.Requests
{
    public class CreateOrderItemRequestDto
    {
        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("aditionals")]
        public List<CreateOrderItemAditionalRequestDto> Aditionals { get; set; } = new();
    }
}
