namespace Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs
{
    /// <summary>
    /// DTO para filtro de permissões
    /// </summary>
    public class PermissionFilterDTO
    {
        /// <summary>
        /// ID do tenant para filtrar
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// ID do módulo para filtrar
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// Nome do módulo para filtrar
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// Status ativo/inativo
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Número da página para paginação
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Tamanho da página para paginação
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
