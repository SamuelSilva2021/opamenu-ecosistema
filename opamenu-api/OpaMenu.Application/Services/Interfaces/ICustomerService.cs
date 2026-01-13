using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Infrastructure.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseDTO<IEnumerable<CustomerResponseDto>>>GetAll();
        Task<ResponseDTO<CustomerResponseDto?>> GetPublicCustomer(string slug, string phoneNumber);
        Task<ResponseDTO<CustomerResponseDto?>> CreatePublicCustomerAsync(CreateCustomerRequestDto request, string slug);
        Task<ResponseDTO<CustomerResponseDto?>> UpdatePublicCustomerAsync(UpdateCustomerRequestDto request, string slug);
    }
}

