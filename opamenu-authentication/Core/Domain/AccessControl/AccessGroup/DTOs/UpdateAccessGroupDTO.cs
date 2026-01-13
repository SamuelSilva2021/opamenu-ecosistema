namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs
{
    /// <summary>
    /// DTO para atualização de grupo de acesso
    /// </summary>
    public class UpdateAccessGroupDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? GroupTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}
