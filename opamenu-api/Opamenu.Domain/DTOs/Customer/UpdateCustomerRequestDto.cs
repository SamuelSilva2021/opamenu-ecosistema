using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs.Customer;

public class UpdateCustomerRequestDto
{
    public Guid Id { get; set; }

    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string? Name { get; set; }

    [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string? Phone { get; set; }

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
