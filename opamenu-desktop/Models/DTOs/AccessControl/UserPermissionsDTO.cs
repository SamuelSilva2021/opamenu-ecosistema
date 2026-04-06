using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl
{
    public class UserPermissionsDTO
    {
        public Guid UserId { get; set; }
        public List<AccessGroupBasicDTO> AccessGroups { get; set; } = new();
    }
}
