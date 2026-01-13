using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    /// <summary>
    /// DTO para alteração de senha
    /// </summary>
    public class UserAccountChangePasswordDTO
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
