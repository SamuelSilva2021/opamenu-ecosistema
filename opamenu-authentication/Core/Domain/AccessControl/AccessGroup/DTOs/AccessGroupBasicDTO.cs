using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;

namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs
{
    public class AccessGroupBasicDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public List<RolesBasicDTO> Roles { get; set; } = new();
    }
}

