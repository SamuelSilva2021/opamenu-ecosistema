using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface ITenantBusinnessService
    {
        Task<ResponseDTO<TenantBusinessResponseDto?>> GetTenantBusinessInfoByTenantId();
        Task<ResponseDTO<TenantBusinessResponseDto>> UpdateTenantBusinessInfo(UpdateTenantBusinessRequestDto dto);
    }
}
