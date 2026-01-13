using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    public class AddressDto
    {
        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Number { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Complement { get; set; }

        [Required]
        [StringLength(50)]
        public string Neighborhood { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        public string State { get; set; } = string.Empty;
    }
}
