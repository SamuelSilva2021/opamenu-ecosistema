using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.BankDetails;
using Opamenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Application.Services.Interfaces.Opamenu
{
    public interface IBankDetailsService
    {
        Task<ResponseDTO<IEnumerable<BankDetailsDto>>> GetAllAsync();
        Task<ResponseDTO<BankDetailsDto>> GetByIdAsync(Guid id);
        Task<ResponseDTO<BankDetailsDto>> CreateAsync(CreateBankDetailsRequestDto request);
        Task<ResponseDTO<BankDetailsDto>> UpdateAsync(UpdateBankDetailsRequestDto request);
        Task <ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
