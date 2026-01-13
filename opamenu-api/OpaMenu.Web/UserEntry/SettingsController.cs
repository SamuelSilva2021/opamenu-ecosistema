using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Tenant;

namespace OpaMenu.Web.UserEntry
{
    [Route("api/settings")]
    [ApiController]
    [Authorize]
    public class SettingsController(ITenantBusinnessService tenantBusinessService) : BaseController
    {
        /// <summary>
        /// Obtém as configurações do cliente (Tenant) atual
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetSettings()
        {
            var response = await tenantBusinessService.GetTenantBusinessInfoByTenantId();
            return BuildResponse(response);
        }

        /// <summary>
        /// Atualiza as configurações do cliente (Tenant) atual
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateSettings([FromBody] UpdateTenantBusinessRequestDto dto)
        {
            var response = await tenantBusinessService.UpdateTenantBusinessInfo(dto);
            return BuildResponse(response);
        }
    }
}
