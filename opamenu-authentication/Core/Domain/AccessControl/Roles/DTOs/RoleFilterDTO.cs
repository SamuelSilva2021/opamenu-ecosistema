namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs
{
    /// <summary>
    /// DTO para filtros de listagem de Roles
    /// </summary>
    public class RoleFilterDTO
    {
        /// <summary>
        /// Filtra por Tenant
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Filtra por Aplicação
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Filtra por Status
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filtra por nome (contém)
        /// </summary>
        public string? NameContains { get; set; }

        /// <summary>
        /// Filtra por código exato
        /// </summary>
        public string? Code { get; set; }
    }
}