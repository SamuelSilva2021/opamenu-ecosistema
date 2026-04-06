using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs.AccessControl
{
    public class ModuleBasicDTO
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public List<string> Operations { get; set; } = new();
    }
}
