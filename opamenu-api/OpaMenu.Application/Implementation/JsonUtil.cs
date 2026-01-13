using OpaMenu.Application.CrossCutting;
using OpaMenu.Application.Implementation.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpaMenu.Application.Implementation
{
    public static class JsonUtil
    {
        /// <summary>
        /// Remove Sensitive data from a json string. Only work with valid json.
        /// </summary>
        /// <param name="jsonString">Json String to process.</param>
        /// <returns>Json String without sensitive data.</returns>
        public static string? RemoveSensitiveDataFromJsonString(string? jsonString)
        {
            try
            {
                return jsonString != null
                    ? RemoveSensitiveDataFromJsonNode(jsonString.ParseJson<JsonNode>()).ToJson()
                    : null;
            }
            catch (Exception)
            {
                // Isn't a valid Json.
                return jsonString;
            }
        }

        /// <summary>
        /// Remove Sensitive Data from a Json Object.
        /// </summary>
        /// <param name="jsonObject">Json Object</param>
        /// <returns></returns>
        public static JsonObject? RemoveSensitiveData(JsonObject? jsonObject) =>
            (JsonObject?)RemoveSensitiveDataFromJsonNode(jsonObject);

        /// <summary>
        /// Remove Sensitive Data from a Json Node.
        /// </summary>
        /// <param name="jsonNode">Json Object</param>
        /// <returns></returns>
        public static JsonNode? RemoveSensitiveDataFromJsonNode(JsonNode? jsonNode)
        {
            var jsonNodeCopy = jsonNode?.ToJson()?.ParseJson<JsonNode>();

            return jsonNodeCopy?.Visit(node =>
            {
                // Note: In this case we don't remove cardnumber, cpf, cnpj. If should be remove, use SensitivePropertyValidator.ShouldMask to evaluate.
                if (node.Parent is JsonObject parent && SensitivePropertyValidator.IsSensitive(node.GetPropertyName()))
                {
                    parent.Remove(node.GetPropertyName());
                }
                return node;
            });
        }


        /// <summary>
        /// Mask Sensitive data from a json string. 
        /// Only work with valid json. 
        /// Use this method only for logging.
        /// </summary>
        /// <param name="jsonString">Json String to process.</param>
        /// <returns>Json String with masked sensitive data.</returns>
        public static string? MaskSensitiveDataFromJsonString(string? jsonString)
        {
            try
            {
                return jsonString != null
                    ? MaskSensitiveDataFromJsonNode(jsonString.ParseJson<JsonNode>()).ToJson()
                    : null;
            }
            catch (Exception)
            {
                // Isn't a valid Json.
                return jsonString;
            }
        }

        /// <summary>
        /// Mask Sensitive Data in a JsonObject. Use this method only for logging.
        /// </summary>
        /// <param name="jsonObject">Json Object to Mask</param>
        /// <returns>New Masked Json Object.</returns>
        public static JsonObject? MaskSensitiveData(JsonObject? jsonObject) =>
            (JsonObject?)MaskSensitiveDataFromJsonNode(jsonObject);


        public static JsonNode? MaskSensitiveDataFromJsonNode(JsonNode? jsonNode)
        {
            var jsonNodeCopy = jsonNode?.ToJson()?.ParseJson<JsonNode>();

            return jsonNodeCopy?.Visit(node =>
            {
                if (node.Parent is JsonObject && SensitivePropertyValidator.ShouldMask(node.GetPropertyName()))
                {
                    if (node is JsonArray || node is JsonObject)
                        node.ReplaceWith(SensitivePropertyReplacer.DEFAULT_REPLACEMENT_MASK);
                    else
                        node.ReplaceWith(SensitivePropertyReplacer.ToMaskedString(node.GetPropertyName(), node.GetValue<string>()));
                }

                return node;
            });
        }
    }
}
