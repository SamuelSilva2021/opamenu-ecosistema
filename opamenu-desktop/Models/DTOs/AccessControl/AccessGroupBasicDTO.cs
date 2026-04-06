using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl
{
    public class AccessGroupBasicDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public List<RolesBasicDTO> Roles { get; set; } = new();
    }
}
