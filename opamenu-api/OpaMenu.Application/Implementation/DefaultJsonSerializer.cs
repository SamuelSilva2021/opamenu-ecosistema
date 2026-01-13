using OpaMenu.Application.Implementation;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpaMenu.Application.CrossCutting
{
    public interface IJsonSerializer
    {
        string? Serialize<T>(T value);

        T? Deserialize<T>(string? json);
    }

    public class DefaultJsonSerializer : IJsonSerializer
    {
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        public static readonly DefaultJsonSerializer JsonSerializer = new(Options);

        private readonly JsonSerializerOptions _options;

        private DefaultJsonSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public string? Serialize<T>(T value) =>
            value != null ? OpaMenuJson.Serialize(value, _options) : null;

        public T? Deserialize<T>(string? json) =>
            json != null ? OpaMenuJson.Deserialize<T>(json, _options) : default;

    }

    public static class DefaultJsonSerializerExtensions
    {
        public static string? ToJson<T>(this T? value) =>
            DefaultJsonSerializer.JsonSerializer.Serialize(value);

        public static T? ParseJson<T>(this string? json) where T : class =>
            DefaultJsonSerializer.JsonSerializer.Deserialize<T>(json);
    }
}
