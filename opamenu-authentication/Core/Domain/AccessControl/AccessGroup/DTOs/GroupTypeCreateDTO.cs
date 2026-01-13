namespace Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs
{
    public class GroupTypeCreateDTO
    {
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
        public bool IsActive { get; set; } = true;
    }
}
