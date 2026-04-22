using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Auth
{
    public class RolePermissionsAggregate
    {
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public List<PermissionAggregate> Permissions { get; set; } = [];
    }
}
