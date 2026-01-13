namespace Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs
{
    /// <summary>
    /// DTO para atualização de relacionamento entre permissão e operação
    /// </summary>
    public class PermissionOperationUpdateDTO
    {
        /// <summary>
        /// Se a operação está ativa na permissão
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
