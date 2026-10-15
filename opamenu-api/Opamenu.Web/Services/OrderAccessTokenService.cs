using System.Security.Cryptography;
using System.Text;

namespace Opamenu.Web.Services;

public class OrderAccessTokenService
{
    private readonly byte[] _key;

    public OrderAccessTokenService(IConfiguration configuration)
    {
        var secret = configuration["Authentication:JwtSecret"];
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("Authentication:JwtSecret não configurado.");

        _key = SHA256.HashData(Encoding.UTF8.GetBytes($"order-access:{secret}"));
    }

    public string Generate(Guid orderId)
    {
        var mac = HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes(orderId.ToString("N")));
        return Convert.ToBase64String(mac).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public bool IsValid(Guid orderId, string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var expected = Encoding.UTF8.GetBytes(Generate(orderId));
        var provided = Encoding.UTF8.GetBytes(token);
        return CryptographicOperations.FixedTimeEquals(expected, provided);
    }
}
