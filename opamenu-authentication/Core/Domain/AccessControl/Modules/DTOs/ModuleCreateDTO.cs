using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    /// <summary>
    /// DTO para criação de um novo módulo
    /// </summary>
    public class ModuleCreateDTO
    {
        /// <summary>
        /// Nome do módulo
        /// </summary>
        [Required(ErrorMessage = "O nome do módulo é obrigatório")]
        [MaxLength(255, ErrorMessage = "O nome do módulo não pode exceder 255 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do módulo
        /// </summary>
        [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL do módulo
        /// </summary>
        [MaxLength(500, ErrorMessage = "A URL não pode exceder 500 caracteres")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Chave única do módulo para identificação
        /// </summary>
        [MaxLength(100, ErrorMessage = "A chave do módulo não pode exceder 100 caracteres")]
        [Required(ErrorMessage = "A chave do módulo é obrigatória")]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Código numérico do módulo
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// ID da aplicação à qual o módulo pertence
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Se o módulo está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
