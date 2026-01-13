using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs
{
    /// <summary>
    /// DTO para atualização de uma conta de usuário existente
    /// </summary>
    public class UserAccountUpdateDTO
    {
        [MaxLength(100, ErrorMessage = "O nome de usuário não pode exceder 100 caracteres")]
        public string? Username { get; set; }

        [MaxLength(255, ErrorMessage = "O email não pode exceder 255 caracteres")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [MaxLength(100, ErrorMessage = "O primeiro nome não pode exceder 100 caracteres")]
        public string? FirstName { get; set; }

        [MaxLength(100, ErrorMessage = "O sobrenome não pode exceder 100 caracteres")]
        public string? LastName { get; set; }

        [MaxLength(20, ErrorMessage = "O número de telefone não pode exceder 20 caracteres")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string? PhoneNumber { get; set; }

        public EUserAccountStatus? Status { get; set; }
        public bool? IsEmailVerified { get; set; }
        public Guid? TenantId { get; set; }
    }
}

