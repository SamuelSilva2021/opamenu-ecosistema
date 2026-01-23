using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Web.UserEntry.Http;
using OpaMenu.Web.UserEntry;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;

namespace OpaMenu.Web.UserEntry.Products;

/// <summary>
/// Controller para gerenciamento de grupos de adicionais de produtos seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/products/{productId:int}/addon-groups")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class ProductAddonGroupsController(
    IProductAddonGroupService productAddonGroupService,
    IProductAddonGroupMapper productAddonGroupMapper,
    ILogger<ProductAddonGroupsController> logger) : BaseController
{
    private readonly IProductAddonGroupService _productAddonGroupService = productAddonGroupService;
    private readonly IProductAddonGroupMapper _productAddonGroupMapper = productAddonGroupMapper;
    private readonly ILogger<ProductAddonGroupsController> _logger = logger;

    /// <summary>
    /// Obter grupos de adicionais de um produto
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductAddonGroupResponseDto>>>> GetProductAddonGroups(Guid productId)
    {
        var serviceResponse = await _productAddonGroupService.GetProductAddonGroupsAsync(productId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Obter produto com todos os grupos de adicionais
    /// </summary>
    [HttpGet("~/api/products/{productId:int}/with-addons")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<ProductWithAddonsResponseDto>>> GetProductWithAddons(Guid productId)
    {
        var serviceResponse = await _productAddonGroupService.GetProductWithAddonsAsync(productId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Adicionar grupo de adicionais a um produto
    /// </summary>
    [HttpPost]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductAddonGroupResponseDto>>> AddAddonGroupToProduct(
        Guid productId, 
        [FromBody] AddProductAddonGroupRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", ModelState));

        var serviceResponse = await _productAddonGroupService.AddAddonGroupToProductAsync(productId, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualizar configuração de grupo de adicionais em um produto
    /// </summary>
    [HttpPut("{addonGroupId:int}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<ProductAddonGroupResponseDto>>> UpdateProductAddonGroup(
        Guid productId,
        Guid addonGroupId, 
        [FromBody] UpdateProductAddonGroupRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", ModelState));

        var serviceResponse = await _productAddonGroupService.UpdateProductAddonGroupAsync(productId, addonGroupId, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Remover grupo de adicionais de um produto
    /// </summary>
    [HttpDelete("{addonGroupId:int}")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse>> RemoveAddonGroupFromProduct(Guid productId, Guid addonGroupId)
    {
        var serviceResponse = await _productAddonGroupService.RemoveAddonGroupFromProductAsync(productId, addonGroupId);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Reordenar grupos de adicionais de um produto
    /// </summary>    //[HttpPut("reorder")]
    //public async Task<ActionResult<ApiResponse>> ReorderProductAddonGroups(
    //    Guid productId, 
    //    [FromBody] Dictionary<int, int> groupOrders)
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", ModelState));

    //    var serviceResponse = await _productAddonGroupService.ReorderProductAddonGroupsAsync(productId, groupOrders);
    //    return BuildResponse(serviceResponse);
    //}


    /// <summary>
    /// Adicionar múltiplos grupos de adicionais a um produto
    /// </summary>
    [HttpPost("bulk")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductAddonGroupResponseDto>>>> BulkAddAddonGroupsToProduct(
        Guid productId, 
        [FromBody] IEnumerable<AddProductAddonGroupRequestDto> requests)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", ModelState));

        var serviceResponse = await _productAddonGroupService.BulkAddAddonGroupsToProductAsync(productId, requests);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Remover múltiplos grupos de adicionais de um produto
    /// </summary>
    [HttpDelete("bulk")]
    [MapPermission(MODULE_PRODUCT, OPERATION_UPDATE)]
    public async Task<ActionResult<ApiResponse>> BulkRemoveAddonGroupsFromProduct(
        Guid productId, 
        [FromBody] IEnumerable<Guid> addonGroupIds)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", ModelState));

        var serviceResponse = await _productAddonGroupService.BulkRemoveAddonGroupsFromProductAsync(productId, addonGroupIds);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Verificar se um grupo de adicionais está associado a um produto
    /// </summary>
    [HttpGet("{addonGroupId:int}/exists")]
    [MapPermission(MODULE_PRODUCT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<bool>>> IsAddonGroupAssignedToProduct(Guid productId, Guid addonGroupId)
    {
        var serviceResponse = await _productAddonGroupService.IsAddonGroupAssignedToProductAsync(productId, addonGroupId);
        return BuildResponse(serviceResponse);
    }
}
