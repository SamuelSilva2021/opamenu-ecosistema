using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

/// <summary>
/// Requisição para cadastro de novo tenant (cliente/empresa)
/// </summary>
public class CreateTenantDTO
{
    /// <summary>
    /// Nome da empresa/organização
    /// </summary>
    [Required(ErrorMessage = "Nome da empresa é obrigatório")]
    [MaxLength(255, ErrorMessage = "Nome da empresa não pode exceder 255 caracteres")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ para empresas ou CPF para MEI/pessoa física
    /// </summary>
    [MaxLength(18, ErrorMessage = "CNPJ/CPF não pode exceder 18 caracteres")]
    public string? Document { get; set; }

    /// <summary>
    /// Razão social da empresa
    /// </summary>
    [MaxLength(255, ErrorMessage = "Razão social não pode exceder 255 caracteres")]
    public string? RazaoSocial { get; set; }

    /// <summary>
    /// Telefone da empresa
    /// </summary>
    [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
    public string? Phone { get; set; }

    /// <summary>
    /// Email corporativo
    /// </summary>
    [EmailAddress(ErrorMessage = "Email corporativo deve ser válido")]
    [MaxLength(255, ErrorMessage = "Email corporativo não pode exceder 255 caracteres")]
    public string? CompanyEmail { get; set; }

    /// <summary>
    /// Website da empresa
    /// </summary>
    [MaxLength(255, ErrorMessage = "Website não pode exceder 255 caracteres")]
    public string? Website { get; set; }

    /// <summary>
    /// Primeiro nome do usuário administrador
    /// </summary>
    [Required(ErrorMessage = "Primeiro nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Primeiro nome não pode exceder 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Último nome do usuário administrador
    /// </summary>
    [Required(ErrorMessage = "Último nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Último nome não pode exceder 100 caracteres")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário administrador
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ser válido")]
    [MaxLength(255, ErrorMessage = "Email não pode exceder 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário administrador
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
    /// Telefone do usuário administrador
    /// </summary>
    [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
    public string? UserPhone { get; set; }

    /// <summary>
    /// CEP do endereço
    /// </summary>
    [MaxLength(10, ErrorMessage = "CEP não pode exceder 10 caracteres")]
    public string? AddressZipcode { get; set; }

    /// <summary>
    /// Logradouro
    /// </summary>
    [MaxLength(255, ErrorMessage = "Logradouro não pode exceder 255 caracteres")]
    public string? AddressStreet { get; set; }

    /// <summary>
    /// Número do endereço
    /// </summary>
    [MaxLength(20, ErrorMessage = "Número não pode exceder 20 caracteres")]
    public string? AddressNumber { get; set; }

    /// <summary>
    /// Complemento do endereço
    /// </summary>
    [MaxLength(100, ErrorMessage = "Complemento não pode exceder 100 caracteres")]
    public string? AddressComplement { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    [MaxLength(100, ErrorMessage = "Bairro não pode exceder 100 caracteres")]
    public string? AddressNeighborhood { get; set; }

    /// <summary>
    /// Cidade
    /// </summary>
    [MaxLength(100, ErrorMessage = "Cidade não pode exceder 100 caracteres")]
    public string? AddressCity { get; set; }

    /// <summary>
    /// Estado (UF)
    /// </summary>
    [MaxLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
    public string? AddressState { get; set; }

    /// <summary>
    /// Fonte do lead (website, facebook, google, indicacao)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Fonte do lead não pode exceder 100 caracteres")]
    public string? LeadSource { get; set; }

    /// <summary>
    /// UTM Source para tracking
    /// </summary>
    [MaxLength(100, ErrorMessage = "UTM Source não pode exceder 100 caracteres")]
    public string? UtmSource { get; set; }

    /// <summary>
    /// UTM Campaign para tracking
    /// </summary>
    [MaxLength(100, ErrorMessage = "UTM Campaign não pode exceder 100 caracteres")]
    public string? UtmCampaign { get; set; }

    /// <summary>
    /// UTM Medium para tracking
    /// </summary>
    [MaxLength(100, ErrorMessage = "UTM Medium não pode exceder 100 caracteres")]
    public string? UtmMedium { get; set; }
}