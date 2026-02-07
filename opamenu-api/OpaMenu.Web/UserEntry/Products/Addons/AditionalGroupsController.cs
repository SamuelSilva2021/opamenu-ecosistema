using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.AditionalGroup;

namespace OpaMenu.Web.UserEntry.Products.Addons;

/// <summary>
/// Controller para gerenciamento de grupos de adicionais seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/aditionalgroups")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class AditionalGroupsController(
    IAditionalGroupService aditionalGroupService,
    ILogger<AditionalGroupsController> logger) : BaseController
{
    private readonly ILogger<AditionalGroupsController> _logger = logger;

    /// <summary>
    /// Obter todos os grupos de adicionais
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<IEnumerable<AditionalGroupResponseDto>>>> GetAditionalGroups()
    {
        var serviceResponse = await aditionalGroupService.GetAllAditionalGroupsAsync();
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter grupo de adicionais por ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<AditionalGroupResponseDto>>> GetAditionalGroup(Guid id)
    {
        var serviceResponse = await aditionalGroupService.GetAditionalGroupWithAditionalsAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Criar novo grupo de adicionais
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_INSERT)]
    public async Task<ActionResult<ResponseDTO<AditionalGroupResponseDto>>> CreateAditionalGroup([FromBody] CreateAditionalGroupRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<AditionalGroupResponseDto>.BuildError(errors));
        }

        var serviceResponse = await aditionalGroupService.CreateAditionalGroupAsync(request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar grupo de adicionais existente
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_UPDATE)]
    public async Task<ActionResult<ResponseDTO<AditionalGroupResponseDto>>> UpdateAditionalGroup(Guid id, [FromBody] UpdateAditionalGroupRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<AditionalGroupResponseDto>.BuildError(errors));
        }

        var serviceResponse = await aditionalGroupService.UpdateAditionalGroupAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Alternar status ativo/inativo do grupo de adicionais
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_UPDATE)]
    public async Task<ActionResult<ResponseDTO<AditionalGroupResponseDto>>> ToggleAditionalGroupStatus(Guid id)
    {
        var serviceResponse = await aditionalGroupService.ToggleAditionalGroupStatusAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Excluir grupo de adicionais
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(MODULE_ADITIONAL_GROUP, OPERATION_DELETE)]
    public async Task<ActionResult<ResponseDTO<bool>>> DeleteAditionalGroup(Guid id)
    {
        var serviceResponse = await aditionalGroupService.DeleteAditionalGroupAsync(id);
        return BuildResponse(serviceResponse);
    }
}
