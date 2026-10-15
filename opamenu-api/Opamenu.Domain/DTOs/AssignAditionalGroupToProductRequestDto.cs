using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Domain.DTOs
{
    public class AssignAditionalGroupToProductRequestDto
    {
        public int DisplayOrder { get; set; } = 0;
        public bool IsRequired { get; set; } = false;
        public int? MinSelectionsOverride { get; set; }
        public int? MaxSelectionsOverride { get; set; }
    }
}
