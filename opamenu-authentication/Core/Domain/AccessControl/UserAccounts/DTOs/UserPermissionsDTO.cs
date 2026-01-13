using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    public class UserPermissionsDTO
    {
        public Guid UserId { get; set; }
        public List<AccessGroupBasicDTO> AccessGroups { get; set; } = new();
    }
}
