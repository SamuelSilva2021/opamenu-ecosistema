using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    public class UserAccountDTO
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public EUserAccountStatus Status { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}

