using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    /// <summary>
    /// DTO para criação de uma nova conta de usuário
    /// </summary>
    public class UserAccountCreateDTO
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [MaxLength(255, ErrorMessage = "O email não pode exceder 255 caracteres")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário (mínimo de 6 caracteres)
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmação da senha (deve coincidir com a senha)
        /// </summary>
        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Primeiro nome do usuário
        /// </summary>
        [MaxLength(100, ErrorMessage = "O primeiro nome não pode exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [MaxLength(100, ErrorMessage = "O sobrenome não pode exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone do usuário
        /// </summary>
        [MaxLength(20, ErrorMessage = "O número de telefone não pode exceder 20 caracteres")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// ID do tenant associado
        /// </summary>
        public Guid TenantId { get; set; }
    }
}
