using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.TenantPaymentConfig;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;

namespace OpaMenu.Web.UserEntry
{
    [Route("api/tenant-payment-config")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class TenantPaymentConfigController(ITenantPaymentConfigService service) : BaseController
    {
        /// <summary>
        /// Obtém a configuração de pagamento PIX ativa do tenant
        /// </summary>
        [HttpGet("pix")]
        [MapPermission(MODULE_SETTINGS, OPERATION_SELECT)]
        public async Task<ActionResult> GetPixConfig()
        {
            var result = await service.GetConfigAsync();
            return BuildResponse(result);
        }

        /// <summary>
        /// Cria ou atualiza a configuração de pagamento
        /// </summary>
        [HttpPost]
        [MapPermission(MODULE_SETTINGS, OPERATION_UPDATE)]
        public async Task<ActionResult> UpsertConfig([FromBody] UpsertTenantPaymentConfigRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.UpsertConfigAsync(dto);
            return BuildResponse(result);
        }
    }
}
