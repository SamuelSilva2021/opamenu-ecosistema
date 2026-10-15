using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Domain.DTOs.Auth
{
    public class RefreshTokenPayload
    {
        public Guid UserId { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
