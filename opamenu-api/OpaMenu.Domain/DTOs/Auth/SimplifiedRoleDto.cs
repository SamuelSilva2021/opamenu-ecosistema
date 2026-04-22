using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Auth
{
    public sealed class SimplifiedRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SimplifiedPermissionDto> Permissions { get; set; } = [];
    }
}
