using Authenticator.API.Core.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authenticator.API.Infrastructure.CrossCutting.Utils
{
    /// <summary>
    /// Implementação padrão do serializador/deserializador JSON
    /// </summary>
    public class DefaultJsonSerializer : IDefaultJsonSerializer
    {
        /// <summary>
        /// Opções padrão para serialização/deserialização JSON
        /// </summary>
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = {new JsonStringEnumConverter()}
        };

        public static readonly DefaultJsonSerializer JsonSerializer = new(Options);

        private readonly JsonSerializerOptions _options;
        public DefaultJsonSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? Options;
        }
        public string? Serialize<T>(T value) => 
            value != null ? JsonSerializer.Serialize(value) : null;
        public T? Deserialize<T>(string? json) =>
            json != null ? JsonSerializer.Deserialize<T>(json) : default;

    }

    /// <summary>
    /// Extensões para facilitar o uso do serializador/deserializador JSON
    /// </summary>
    public static class DefaultJsonSerializerExtensions
    {
        public static string? ToJson<T>(this T value) => 
            DefaultJsonSerializer.JsonSerializer.Serialize(value);
        public static T? ParseJson<T>(this string? json) where T : class => 
            DefaultJsonSerializer.JsonSerializer.Deserialize<T>(json);
    }
}
