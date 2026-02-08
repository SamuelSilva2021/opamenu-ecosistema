using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    /// <summary>
    /// DTO para listagem de usuários
    /// </summary>
    public class UserAccountListDTO
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public EUserAccountStatus Status { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}

