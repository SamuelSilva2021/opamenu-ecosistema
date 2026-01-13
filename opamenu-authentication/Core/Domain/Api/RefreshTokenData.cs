namespace Authenticator.API.Core.Domain.Api
{
    /// <summary>
    /// Dados armazenados em um refresh token
    /// </summary>
    public class RefreshTokenData
    {
        public Guid UserId { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
