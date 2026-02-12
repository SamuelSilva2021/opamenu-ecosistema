using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.TenantPaymentMethod;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Web.UserEntry
{
    [Route("api/tenant-payment-methods")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class TenantPaymentMethodsController(ITenantPaymentMethodService service) : BaseController
    {
        /// <summary>
        /// Obtém os métodos de pagamento configurados para o tenant
        /// </summary>
        [HttpGet]
        [MapPermission([MODULE_PDV, MODULE_SETTINGS, ORDER], OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<TenantPaymentMethodResponseDto>>>> GetAll()
        {
            var result = await service.GetAllByTenantAsync();
            return BuildResponse(result);
        }
    }
}
