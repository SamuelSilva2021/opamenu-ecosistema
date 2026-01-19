using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    public class UserAccountWithGroupsDTO : UserAccountDTO
    {
        public IEnumerable<AccessGroupDTO> AccessGroups { get; set; } = new List<AccessGroupDTO>();
    }
}
