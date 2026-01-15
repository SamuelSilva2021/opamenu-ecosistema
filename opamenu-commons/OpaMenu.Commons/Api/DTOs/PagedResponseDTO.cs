using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.DTOs
{
    /// <summary>
    /// Representa uma resposta paginada contendo uma coleção de itens do tipo T, juntamente com informações de paginação.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
