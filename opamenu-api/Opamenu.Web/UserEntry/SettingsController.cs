using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opamenu.Application.Services.Interfaces.Opamenu;
using Opamenu.Domain.DTOs.Tenant;
using Opamenu.Infrastructure.Anotations;
using Opamenu.Infrastructure.Filters;

namespace Opamenu.Web.UserEntry
{
    [Route("api/settings")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class SettingsController(ITenantBusinnessService tenantBusinessService) : BaseController
    {
        /// <summary>
        /// Obtém as configurações do cliente (Tenant) atual
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MapPermission(MODULE_SETTINGS, OPERATION_SELECT)]
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
        [MapPermission(MODULE_SETTINGS, OPERATION_UPDATE)]
        public async Task<ActionResult> UpdateSettings([FromBody] UpdateTenantBusinessRequestDto dto)
        {
            var response = await tenantBusinessService.UpdateTenantBusinessInfo(dto);
            return BuildResponse(response);
        }

        /// <summary>
        /// Verifica se um slug (link do cardápio) está disponível para uso
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("check-slug")]
        [MapPermission(MODULE_SETTINGS, OPERATION_SELECT)] // Or anonymous, but here we require settings view perm
        public async Task<ActionResult> CheckSlugAvailability([FromQuery] string slug)
        {
            var response = await tenantBusinessService.CheckSlugAvailability(slug);
            return BuildResponse(response);
        }
    }
}
