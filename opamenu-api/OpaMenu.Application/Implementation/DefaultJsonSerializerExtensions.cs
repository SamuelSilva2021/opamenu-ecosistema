namespace OpaMenu.Application.CrossCutting
{
    public static class DefaultJsonSerializerExtensions
    {
        public static string? ToJson<T>(this T? value) =>
            DefaultJsonSerializer.JsonSerializer.Serialize(value);

        public static T? ParseJson<T>(this string? json) where T : class =>
            DefaultJsonSerializer.JsonSerializer.Deserialize<T>(json);
    }
}
