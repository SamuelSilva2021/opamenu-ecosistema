using System.Net;
using System.Text.Json.Serialization;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// DTO padrão para respostas paginadas
/// </summary>
/// <typeparam name="T">Tipo do item</typeparam>
public class PagedResponseDTO<T>
{
    /// <summary>
    /// Itens da página atual
    /// </summary>
    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Número da página atual
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página (limite)
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// Total de itens
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Se a operação foi bem-sucedida
    /// </summary>
    [JsonPropertyName("succeeded")]
    public bool? Succeeded { get; set; }
    /// <summary>
    /// Codigo HTTP da resposta
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }
    /// <summary>
    /// Gets or sets the current page number in a paginated collection.
    /// </summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
}