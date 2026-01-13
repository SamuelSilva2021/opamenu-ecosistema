using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Requisição para criação de novo usuário (requer autenticação)
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Primeiro nome do usuário
    /// </summary>
    [Required(ErrorMessage = "Primeiro nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Primeiro nome não pode exceder 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Último nome do usuário
    /// </summary>
    [Required(ErrorMessage = "Último nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Último nome não pode exceder 100 caracteres")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email único do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ser válido")]
    [MaxLength(255, ErrorMessage = "Email não pode exceder 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    [MaxLength(100, ErrorMessage = "Senha não pode exceder 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha
    /// </summary>
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "Confirmação de senha não confere")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do usuário (opcional)
    /// </summary>
    [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
    public string? Phone { get; set; }

    /// <summary>
    /// Nome de usuário (username) - opcional, será gerado se não fornecido
    /// </summary>
    [MaxLength(100, ErrorMessage = "Username não pode exceder 100 caracteres")]
    public string? Username { get; set; }

    /// <summary>
    /// Indica se o usuário deve ser criado como ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lista de IDs de grupos de acesso para associar ao usuário
    /// </summary>
    public List<Guid> AccessGroupIds { get; set; } = new List<Guid>();

    /// <summary>
    /// Notas adicionais sobre o usuário
    /// </summary>
    [MaxLength(500, ErrorMessage = "Notas não podem exceder 500 caracteres")]
    public string? Notes { get; set; }
}