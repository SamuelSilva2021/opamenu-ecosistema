using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Auth
{
    public sealed class AccessGroupBasicDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public List<RoleBasicDto> Roles { get; set; } = [];
    }
}
