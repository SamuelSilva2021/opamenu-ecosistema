using Opamenu.Application.DTOs;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Application.Services.Interfaces.Opamenu
{
    public interface ITenantBusinnessService
    {
        Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoByTenantId();
        Task<ResponseDTO<TenantBusinessResponseDto>> UpdateTenantBusinessInfo(UpdateTenantBusinessRequestDto dto);
        Task<ResponseDTO<bool>> CheckSlugAvailability(string slug);
    }
}
