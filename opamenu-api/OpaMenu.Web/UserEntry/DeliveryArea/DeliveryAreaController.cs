using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.DeliveryArea;
using OpaMenu.Infrastructure.Authentication;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Web.UserEntry.DeliveryArea;

[Authorize]
[ApiController]
[Route("api/delivery-areas")]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class DeliveryAreaController(
    IDeliveryAreaService service,
    ICurrentUserService currentUserService) : BaseController
{
    [HttpGet]
    [MapPermission(MODULE_DELIVERY_AREA, OPERATION_SELECT)]
    public async Task<IActionResult> GetAll()
    {
        var tenantId = currentUserService.GetTenantGuid()!.Value;
        var serviceResponse = await service.GetAllAsync(tenantId);
        return BuildResponse(serviceResponse);
    }

    [HttpGet("{id}")]
    [MapPermission(MODULE_DELIVERY_AREA, OPERATION_SELECT)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenantId = currentUserService.GetTenantGuid()!.Value;
        var serviceResponse = await service.GetByIdAsync(id, tenantId);
        return BuildResponse(serviceResponse);
    }

    [HttpPost]
    [MapPermission(MODULE_DELIVERY_AREA, OPERATION_INSERT)]
    public async Task<IActionResult> Create(CreateDeliveryAreaRequestDto request)
    {
        var tenantId = currentUserService.GetTenantGuid()!.Value;
        var serviceResponse = await service.CreateAsync(request, tenantId);
        return BuildResponse(serviceResponse);
    }

    [HttpPut("{id}")]
    [MapPermission(MODULE_DELIVERY_AREA, OPERATION_UPDATE)]
    public async Task<IActionResult> Update(Guid id, CreateDeliveryAreaRequestDto request)
    {
        var tenantId = currentUserService.GetTenantGuid()!.Value;
        var serviceResponse = await service.UpdateAsync(id, request, tenantId);
        return BuildResponse(serviceResponse);
    }

    [HttpDelete("{id}")]
    [MapPermission(MODULE_DELIVERY_AREA, OPERATION_DELETE)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tenantId = currentUserService.GetTenantGuid()!.Value;
        var serviceResponse = await service.DeleteAsync(id, tenantId);
        return BuildResponse(serviceResponse);
    }
}
