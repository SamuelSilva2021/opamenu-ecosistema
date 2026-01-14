using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Commons
{
    public static class DefaultJsonSerializerExtensions
    {
        public static string? ToJson<T>(this T? value) =>
            DefaultJsonSerializer.JsonSerializer.Serialize(value);

        public static T? ParseJson<T>(this string? json) where T : class =>
            DefaultJsonSerializer.JsonSerializer.Deserialize<T>(json);
    }
}
