using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Commons
{
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
}
