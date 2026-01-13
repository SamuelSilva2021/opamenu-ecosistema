namespace Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs
{
    /// <summary>
    /// DTO para operação de permissão
    /// </summary>
    public class PermissionOperationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
