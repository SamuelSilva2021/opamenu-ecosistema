namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs
{
    /// <summary>
    /// DTO para atualização de tipo de grupo
    /// </summary>
    public class GroupTypeUpdateDTO
    {
        /// <summary>
        /// Nome do tipo de grupo
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Descrição do tipo de grupo
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Código do tipo de grupo
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// Indica se o tipo de grupo está ativo
        /// </summary>
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
