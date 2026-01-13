using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Customer;

public class CreateCustomerRequestDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    public string? PostalCode { get; set; }
    public string? Street { get; set; }
    public string? StreetNumber { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Complement { get; set; }
}
