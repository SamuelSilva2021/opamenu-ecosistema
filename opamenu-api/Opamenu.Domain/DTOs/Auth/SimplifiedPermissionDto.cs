using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Domain.DTOs.Auth
{
    public sealed class SimplifiedPermissionDto
    {
        public string Module { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = [];
    }
}
