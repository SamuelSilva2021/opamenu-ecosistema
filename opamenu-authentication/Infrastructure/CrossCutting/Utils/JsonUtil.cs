using System.Text.Json.Nodes;

namespace Authenticator.API.Infrastructure.CrossCutting.Utils
{
    public static class JsonUtil
    {
        public static JsonNode? StringToJson(string jsonString) 
            => jsonString != null ? JsonNode.Parse(jsonString) : null;
    }
}
