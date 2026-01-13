using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;

namespace Authenticator.API.Core.Domain.Api
{
    /// <summary>
    /// Representa uma linha "achatada" que combina informações de grupos, papéis, módulos e operações.
    /// </summary>
    public class FlatRowDTO
    {
        public Guid GroupId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public Guid ModuleId { get; set; }
        public string ModuleKey { get; set; } = string.Empty;
        public string Operations { get; set; } = string.Empty;
    }
}
