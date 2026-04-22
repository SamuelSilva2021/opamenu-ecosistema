using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Auth
{
    public class PermissionAggregate
    {
        public string ModuleKey { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = [];
    }
}
