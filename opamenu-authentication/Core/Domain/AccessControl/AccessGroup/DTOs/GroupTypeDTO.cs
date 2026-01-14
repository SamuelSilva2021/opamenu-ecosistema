namespace Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs
{
    /// <summary>
    /// DTO de retorno para Tipo de Grupo de Acesso
    /// </summary>
    public class GroupTypeDTO
    {
        /// <summary>
        /// Identificador único do tipo de grupo de acesso
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do tipo de grupo de acesso
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Descrição do tipo de grupo de acesso
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Código do tipo de grupo de acesso (ex: "SYSTEM", "CUSTOM")
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Indica se o tipo de grupo de acesso está ativo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de criação do tipo de grupo de acesso
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização do tipo de grupo de acesso
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
