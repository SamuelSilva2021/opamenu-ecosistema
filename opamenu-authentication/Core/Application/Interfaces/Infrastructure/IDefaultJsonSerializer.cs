namespace Authenticator.API.Core.Application.Interfaces.Infrastructure
{
    /// <summary>
    /// Serializador/Deserializador JSON
    /// </summary>
    public interface IDefaultJsonSerializer
    {
        string? Serialize<T>(T value);
        T? Deserialize<T>(string? json);
    }
}
