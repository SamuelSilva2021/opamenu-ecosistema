using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Application.Implementation
{
    public static class OpaMenuJson
    {
        private static JsonSerializerOptions DefaultJsonSerializerOptions => new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.Default,
            Converters = { new JsonStringEnumConverter() },
            // Using Ignore Cycles to avoid cycles in EF Core entity navigation.
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        private static JsonSerializerOptions IgnoreCyclesJsonSerializerOptions => new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.Default,
            Converters = { new JsonStringEnumConverter() },
            // Using Ignore Cycles to avoid cycles in EF Core entity navigation.
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        /// <summary>
        /// Serialize using default options.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Serialized Json when valid, null when invalid.</returns>
        public static string? Serialize<T>(T value) => Serialize(value, DefaultJsonSerializerOptions);

        /// <summary>
        /// Serialize using default options.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="value">Object to be serialized.</param>
        /// <param name="options">Custom Json Serialize Options.</param>
        /// <returns>Serialized Json when valid, null when invalid.</returns>
        public static string? Serialize<T>(T value, JsonSerializerOptions options) =>
            value != null ? JsonSerializer.Serialize(value, options) : null;


        /// <summary>
        /// Serialize ignoring cycles.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Serialized Json when valid, null when invalid.</returns>
        public static string? SerializeIgnoringCycles<T>(T value) => Serialize(value, IgnoreCyclesJsonSerializerOptions);

        /// <summary>
        /// Deserialize using default options.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="json">Json to be deserialized.</param>
        /// <returns>Serialized Json when valid, null when invalid.</returns>
        public static T? Deserialize<T>(string? json) => Deserialize<T>(json, DefaultJsonSerializerOptions);



        /// <summary>
        /// Des using default options.
        /// </summary>
        /// <typeparam name="T">Object Type.</typeparam>
        /// <param name="json">Json to be deserialized.</param>
        /// <param name="options">Custom Json Serialize Options.</param>
        /// <returns>Serialized Json when valid, null when invalid.</returns>
        public static T? Deserialize<T>(string? json, JsonSerializerOptions options) =>
        json != null ? JsonSerializer.Deserialize<T>(json, options) : default;

        /// <summary>
        /// Json Diff using System.Text.Json.JsonDiffPatch.
        /// </summary>
        /// <param name="first">First Json to compare.</param>
        /// <param name="second">Second Json to compare.</param>
        /// <param name="options">JsonDiffOptions.</param>
        /// <returns>Diff as String when valid, null when invalid.</returns>
        //public static string? JsonDiff(string first, string second, JsonDiffOptions? options = null) =>
        //    JsonDiffPatcher.Diff(first, second, options)?.ToJsonString();
    }
}
