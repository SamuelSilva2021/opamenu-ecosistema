using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl
{
    public class RolesBasicDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public List<ModuleBasicDTO> Modules { get; set; } = new();
    }
}
