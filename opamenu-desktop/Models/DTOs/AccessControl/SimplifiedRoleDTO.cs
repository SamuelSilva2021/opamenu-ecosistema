using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl
{
    public class SimplifiedRoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SimplifiedPermissionDTO> Permissions { get; set; } = [];
    }

    public class SimplifiedPermissionDTO
    {
        public string Module { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = [];
    }
}
