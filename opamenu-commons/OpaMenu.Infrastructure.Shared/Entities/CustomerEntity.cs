using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    public class CustomerEntity
    {
        [Column(name: "id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(name: "name")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(20)]
        [Column(name: "phone")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Column(name: "email")]
        public string? Email { get; set; }
        [Column(name: "postal_code")]
        public string? PostalCode { get; set; }
        [Column(name: "street")]
        public string? Street { get; set; }
        [Column(name: "street_number")]
        public string? StreetNumber { get; set; }
        [Column(name: "neighborhood")]
        public string? Neighborhood { get; set; }
        [Column(name: "city")]
        public string? City { get; set; }
        [Column(name: "state")]
        public string? State { get; set; }
        [Column(name: "complement")]
        public string? Complement { get; set; }
        [Column(name: "created_at")]
        public DateTime CreatedAt { get; set; }
        [Column(name: "updated_at")]
        public DateTime UpdatedAt { get; set; }

        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
        public ICollection<TenantCustomerEntity> TenantCustomers { get; set; } = new List<TenantCustomerEntity>();

    }
}

