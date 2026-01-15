using OpaMenu.Commons.Api.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.DTOs
{
    /// <summary>
    /// Response DTO
    /// Used to response a request.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ResponseDTO<T> : IResponseDTO
    {
        /// <summary>
        /// Identify when operation succeeds.
        /// </summary>
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }

        /// <summary>
        /// Code response.
        /// </summary>
        [JsonIgnore]
        public int Code { get; set; }

        /// <summary>
        /// Error List.
        /// </summary>
        [JsonPropertyName("errors")]
        public IList<ErrorDTO> Errors { get; set; } = new List<ErrorDTO>();

        /// <summary>
        /// Optional response headers. 
        /// </summary>
        [JsonPropertyName("headers")]
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        private dynamic? _data;

        /// <summary>
        /// Optional object data.
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data
        {
            get => _data;
            set => _data = value;
        }

        public dynamic? GetData() => _data;

        /// <summary>
        /// Request Url (optional).
        /// </summary>
        [JsonPropertyName("requestUrl")]
        public string? RequestUrl { get; set; }

        /// <summary>
        /// Request Body (optional).
        /// </summary>
        [JsonPropertyName("requestBody")]
        public string? RequestBody { get; set; }

        /// <summary>
        /// Response returned when request succeeds but return 
        /// a non json response and T type is not string. 
        /// </summary>
        [JsonPropertyName("rawResponseBody")]
        public string? RawResponseBody { get; set; }

        /// <summary>
        /// Serialize Data property to Json.
        /// </summary>
        /// <returns>Json value as string.</returns>
        [JsonIgnore]
        public string? DataAsJson =>
            ((T?)GetData()).ToJson();

        /// <summary>
        /// Serialize Data property to Json masking sensitive data.
        /// </summary>
        /// <returns>Json value as string.</returns>
        [JsonIgnore]
        public string? DataAsSafeJson =>
            JsonUtil.MaskSensitiveDataFromJsonString(DataAsJson);

        public Exception? Exception { get; set; }

    }
}
