using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry.Products.Addons;

/// <summary>
/// Controller para gerenciamento de adicionais seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class AddonsController(
    IAddonService addonService
    ) : BaseController
{
    private readonly IAddonService _addonService = addonService;

    /// <summary>
    /// Obter todos os adicionais
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_ADDON, OPERATION_SELECT)]
    public async Task<ActionResult<IEnumerable<AddonResponseDto>>> GetAddons()
    {
            var serviceResponse = await _addonService.GetAllAddonsAsync();
            return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter adicional por ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission(MODULE_ADDON, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<AddonResponseDto>>> GetAddon(Guid id)
    {
        var serviceResponse = await _addonService.GetAddonByIdAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Criar novo adicional
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_ADDON, OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<AddonResponseDto>>> CreateAddon([FromBody] CreateAddonRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<AddonResponseDto>.ErrorResponse(errors));
        }

        var serviceResponse = await _addonService.CreateAddonAsync(request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar adicional existente
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(MODULE_ADDON, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<AddonResponseDto>>> UpdateAddon(Guid id, [FromBody] UpdateAddonRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<AddonResponseDto>.ErrorResponse(errors));
        }

        var serviceResponse = await _addonService.UpdateAddonAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Excluir adicional
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(MODULE_ADDON, OPERATION_DELETE)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAddon(Guid id)
    {
        var serviceResponse = await _addonService.DeleteAddonAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Alternar status do adicional
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [MapPermission(MODULE_ADDON, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<AddonResponseDto>>> ToggleAddonStatus(Guid id)
    {
        var serviceResponse = await _addonService.ToggleAddonStatusAsync(id);
        return BuildResponse(serviceResponse);
    }
}