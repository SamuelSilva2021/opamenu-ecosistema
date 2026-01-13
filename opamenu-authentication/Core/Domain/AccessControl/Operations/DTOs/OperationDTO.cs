namespace Authenticator.API.Core.Domain.AccessControl.Operations.DTOs
{
    /// <summary>
    /// DTO para resposta de operação
    /// </summary>
    public class OperationDTO
    {
        /// <summary>
        /// ID da operação
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da operação
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da operação
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Valor da operação (ex: 'CREATE', 'READ', 'UPDATE','DELETE')
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Se a operação está ativa
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
