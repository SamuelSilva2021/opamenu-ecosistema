namespace Authenticator.API.Core.Domain.Authentication
{
    public class AuthenticatedUser
    {
        public Guid TenantId { get; set; }
        public string? TenantSlug { get; set; }
        public string? TenantName { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
