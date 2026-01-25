using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry.Products.Addons;

/// <summary>
/// Controller para gerenciamento de grupos de adicionais seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class AddonGroupsController(
    IAddonGroupService addonGroupService,
    ILogger<AddonGroupsController> logger) : BaseController
{
    private readonly IAddonGroupService _addonGroupService = addonGroupService;
    private readonly ILogger<AddonGroupsController> _logger = logger;

    /// <summary>
    /// Obter todos os grupos de adicionais
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<IEnumerable<AddonGroupResponseDto>>>> GetAddonGroups()
    {
        var serviceResponse = await _addonGroupService.GetAllAddonGroupsAsync();
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter grupo de adicionais por ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<AddonGroupResponseDto>>> GetAddonGroup(Guid id)
    {
        var serviceResponse = await _addonGroupService.GetAddonGroupWithAddonsAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Criar novo grupo de adicionais
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_INSERT)]
    public async Task<ActionResult<ResponseDTO<AddonGroupResponseDto>>> CreateAddonGroup([FromBody] CreateAddonGroupRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<AddonGroupResponseDto>.BuildError(errors));
        }

        var serviceResponse = await _addonGroupService.CreateAddonGroupAsync(request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar grupo de adicionais existente
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_UPDATE)]
    public async Task<ActionResult<ResponseDTO<AddonGroupResponseDto>>> UpdateAddonGroup(Guid id, [FromBody] UpdateAddonGroupRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<AddonGroupResponseDto>.BuildError(errors));
        }

        var serviceResponse = await _addonGroupService.UpdateAddonGroupAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Alternar status ativo/inativo do grupo de adicionais
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_UPDATE)]
    public async Task<ActionResult<ResponseDTO<AddonGroupResponseDto>>> ToggleAddonGroupStatus(Guid id)
    {
        var serviceResponse = await _addonGroupService.ToggleAddonGroupStatusAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Excluir grupo de adicionais
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(MODULE_ADDON_GROUP, OPERATION_DELETE)]
    public async Task<ActionResult<ResponseDTO<bool>>> DeleteAddonGroup(Guid id)
    {
        var serviceResponse = await _addonGroupService.DeleteAddonGroupAsync(id);
        return BuildResponse(serviceResponse);
    }
}