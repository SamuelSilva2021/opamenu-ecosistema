using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Auth
{
    public sealed class UserPermissionsDto
    {
        public Guid UserId { get; set; }
        public List<AccessGroupBasicDto> AccessGroups { get; set; } = [];
    }

}
