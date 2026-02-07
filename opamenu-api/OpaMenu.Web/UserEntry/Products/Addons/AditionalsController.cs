using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.Aditionals;

namespace OpaMenu.Web.UserEntry.Products.Addons;

/// <summary>
/// Controller para gerenciamento de adicionais seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/aditionals")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class AditionalsController(
    IAditionalService aditionalService
    ) : BaseController
{
    private readonly IAditionalService _aditionalService = aditionalService;

    /// <summary>
    /// Obter todos os adicionais
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_ADITIONAL, OPERATION_SELECT)]
    public async Task<ActionResult<IEnumerable<AditionalResponseDto>>> GetAditionals()
    {
            var serviceResponse = await _aditionalService.GetAllAditionalsAsync();
            return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter adicional por ID
    /// </summary>
    [HttpGet("{id}")]
    [MapPermission(MODULE_ADITIONAL, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<AditionalResponseDto>>> GetAditional(Guid id)
    {
        var serviceResponse = await _aditionalService.GetAditionalByIdAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Criar novo adicional
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_ADITIONAL, OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<AditionalResponseDto>>> CreateAditional([FromBody] CreateAditionalRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<AditionalResponseDto>.ErrorResponse(errors));
        }

        var serviceResponse = await _aditionalService.CreateAditionalAsync(request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar adicional existente
    /// </summary>
    [HttpPut("{id}")]
    [MapPermission(MODULE_ADITIONAL, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<AditionalResponseDto>>> UpdateAditional(Guid id, [FromBody] UpdateAditionalRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return BadRequest(ApiResponse<AditionalResponseDto>.ErrorResponse(errors));
        }

        var serviceResponse = await _aditionalService.UpdateAditionalAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Excluir adicional
    /// </summary>
    [HttpDelete("{id}")]
    [MapPermission(MODULE_ADITIONAL, OPERATION_DELETE)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAditional(Guid id)
    {
        var serviceResponse = await _aditionalService.DeleteAditionalAsync(id);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Alternar status do adicional
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [MapPermission(MODULE_ADITIONAL, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<AditionalResponseDto>>> ToggleAditionalStatus(Guid id)
    {
        var serviceResponse = await _aditionalService.ToggleAditionalStatusAsync(id);
        return BuildResponse(serviceResponse);
    }
}
