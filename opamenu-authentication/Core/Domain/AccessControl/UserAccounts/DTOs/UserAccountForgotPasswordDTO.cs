using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    /// <summary>
    /// DTO para solicitação de reset de senha
    /// </summary>
    public class UserAccountForgotPasswordDTO
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
    }
}
