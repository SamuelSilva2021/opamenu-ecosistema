using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs
{
    /// <summary>
    /// DTO para criação de grupo de acesso
    /// </summary>
    public class CreateAccessGroupDTO
    {
        [Required (ErrorMessage = "Nome do grupo é obrigatório")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Descrição do grupo é obrigatória")]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public string? Code { get; set; }
        public Guid? TenantId { get; set; }
        [Required(ErrorMessage = "Tipo do grupo é obrigatório")]
        public Guid GroupTypeId { get; set; }
    }
}
