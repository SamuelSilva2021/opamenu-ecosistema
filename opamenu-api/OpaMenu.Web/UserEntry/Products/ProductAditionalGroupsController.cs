using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Web.UserEntry.Http;
using OpaMenu.Web.UserEntry;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry.Products;

/// <summary>
/// Controller para gerenciamento de grupos de adicionais de produtos seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/products/{productId:Guid}/aditional-groups")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class ProductAditionalGroupsController(
    IProductAditionalGroupService productAditionalGroupService,
    IProductAditionalGroupMapper productAditionalGroupMapper,
    ILogger<ProductAditionalGroupsController> logger) : BaseController
{
    private readonly IProductAditionalGroupService _productAditionalGroupService = productAditionalGroupService;
    private readonly IProductAditionalGroupMapper _productAditionalGroupMapper = productAditionalGroupMapper;
    private readonly ILogger<ProductAditionalGroupsController> _logger = logger;

    /// <summary>
    /// Obter grupos de adicionais de um produto
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductAditionalGroupResponseDto>>>> GetProductAditionalGroups(Guid productId)
    {
        var serviceResponse = await _productAditionalGroupService.GetProductAditionalGroupsAsync(productId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter produto com todos os grupos de adicionais
    /// </summary>
    [HttpGet("with-aditionals")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<ProductWithAditionalsResponseDto>>> GetProductWithAditionals(Guid productId)
    {
        var serviceResponse = await _productAditionalGroupService.GetProductWithAditionalsAsync(productId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Adicionar grupo de adicionais a um produto
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductAditionalGroupResponseDto>>> AddAditionalGroupToProduct(
        Guid productId, 
        [FromBody] AddProductAditionalGroupRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados invÃ¡lidos", ModelState));

        var serviceResponse = await _productAditionalGroupService.AddAditionalGroupToProductAsync(productId, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar configuraÃ§Ã£o de grupo de adicionais em um produto
    /// </summary>
    [HttpPut("{aditionalGroupId:Guid}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductAditionalGroupResponseDto>>> UpdateProductAditionalGroup(
        Guid productId,
        Guid aditionalGroupId, 
        [FromBody] UpdateProductAditionalGroupRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados invÃ¡lidos", ModelState));

        var serviceResponse = await _productAditionalGroupService.UpdateProductAditionalGroupAsync(productId, aditionalGroupId, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Remover grupo de adicionais de um produto
    /// </summary>
    [HttpDelete("{aditionalGroupId:Guid}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse>> RemoveAditionalGroupFromProduct(Guid productId, Guid aditionalGroupId)
    {
        var serviceResponse = await _productAditionalGroupService.RemoveAditionalGroupFromProductAsync(productId, aditionalGroupId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Adicionar mÃºltiplos grupos de adicionais a um produto
    /// </summary>
    [HttpPost("bulk")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductAditionalGroupResponseDto>>>> BulkAddAditionalGroupsToProduct(
        Guid productId, 
        [FromBody] IEnumerable<AddProductAditionalGroupRequestDto> requests)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados invÃ¡lidos", ModelState));

        var serviceResponse = await _productAditionalGroupService.BulkAddAditionalGroupsToProductAsync(productId, requests);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Remover mÃºltiplos grupos de adicionais de um produto
    /// </summary>
    [HttpDelete("bulk")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse>> BulkRemoveAditionalGroupsFromProduct(
        Guid productId, 
        [FromBody] IEnumerable<Guid> aditionalGroupIds)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados invÃ¡lidos", ModelState));

        var serviceResponse = await _productAditionalGroupService.BulkRemoveAditionalGroupsFromProductAsync(productId, aditionalGroupIds);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Verificar se um grupo de adicionais estÃ¡ associado a um produto
    /// </summary>
    [HttpGet("{aditionalGroupId:Guid}/exists")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<bool>>> IsAditionalGroupAssignedToProduct(Guid productId, Guid aditionalGroupId)
    {
        var serviceResponse = await _productAditionalGroupService.IsAditionalGroupAssignedToProductAsync(productId, aditionalGroupId);
        return BuildResponse(serviceResponse);
    }
}
