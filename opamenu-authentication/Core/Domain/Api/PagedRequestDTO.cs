using System.Text.Json.Serialization;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// DTO padrão para requisições paginadas
/// </summary>
public class PagedRequestDTO
{
    /// <summary>
    /// Número da página (inicia em 1)
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10;
}