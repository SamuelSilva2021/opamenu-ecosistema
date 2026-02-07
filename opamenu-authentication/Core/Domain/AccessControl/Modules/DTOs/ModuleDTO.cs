
namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    /// <summary>
    /// DTO para resposta de módulo
    /// </summary>
    public class ModuleDTO
    {
        /// <summary>
        /// ID único do módulo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do módulo
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do módulo
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL do módulo
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Chave única do módulo para identificação
        /// </summary>
        public string? Key { get; set; } 

        /// <summary>
        /// Código numérico do módulo
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// ID da aplicação à qual o módulo pertence
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// ID do tipo de módulo
        /// </summary>
        public Guid? ModuleTypeId { get; set; }

        /// <summary>
        /// Nome do tipo de módulo
        /// </summary>
        public string? ModuleTypeName { get; set; }

        /// <summary>
        /// Nome da aplicação
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Se o módulo está ativo
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
