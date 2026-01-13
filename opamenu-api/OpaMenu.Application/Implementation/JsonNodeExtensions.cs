using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpaMenu.Application.Implementation
{
    public static class JsonNodeExtensions
    {
        public static JsonNode? Visit(this JsonNode? jsonNode, Func<JsonNode, JsonNode> visitor)
        {
            if (jsonNode is null) return null;

            if (jsonNode is JsonValue jsonValue)
            {
                return visitor.Invoke(jsonValue);
            }

            if (jsonNode is JsonObject jsonObject)
            {
                var keys = jsonObject.Select(p => p.Key).ToList();
                foreach (var key in keys)
                {
                    Visit(jsonObject[key], visitor);
                }

                visitor.Invoke(jsonObject);

                return jsonObject;
            }

            if (jsonNode is JsonArray jsonArray)
            {
                foreach (var element in jsonArray)
                {
                    Visit(element, visitor);
                }

                visitor.Invoke(jsonArray);

                return jsonArray;
            }

            throw new ArgumentException($"Error visiting unsupported JsonNode type {jsonNode.GetType()}");
        }

    }
}
