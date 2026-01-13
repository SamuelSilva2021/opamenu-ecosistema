using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Operations.DTOs
{
    /// <summary>
    /// DTO para criação de operação
    /// </summary>
    public class OperationCreateDTO
    {
        /// <summary>
        /// Nome da operação
        /// </summary>
        [Required(ErrorMessage = "O nome da operação é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da operação
        /// </summary>
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// Valor da operação (ex: 'CREATE', 'READ', 'UPDATE','DELETE')
        /// </summary>
        [Required(ErrorMessage = "O valor da operação é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O valor deve ter entre 2 e 50 caracteres")]
        public string? Value { get; set; }

        /// <summary>
        /// Se a operação está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
