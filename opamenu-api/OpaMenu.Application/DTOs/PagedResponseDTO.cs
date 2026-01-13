using System.Text.Json.Serialization;

namespace OpaMenu.Application.DTOs
{
    public class PagedResponseDTO<T> : ResponseDTO<IEnumerable<T>>
    {
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }
}