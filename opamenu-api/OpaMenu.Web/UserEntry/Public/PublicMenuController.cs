using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Common.Interfaces;
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Services;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Domain.DTOs.Menu;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Domain.DTOs.Tenant;
using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Web.UserEntry;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Web.UserEntry.Public;

[ApiController]
[Route("api/public/{slug}")]
[AllowAnonymous]
public class PublicMenuController(
    IProductService productService, 
    ICategoryService categoryService, 
    ITenantService tenantService, 
    IStorefrontService storefrontService, 
    ICouponService couponService,
    IOrderService orderService,
    ICustomerService customerService,
    IPaymentService paymentService
    ) : BaseController
{
    private readonly IProductService _productService = productService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly ITenantService _tenantService = tenantService;
    private readonly IStorefrontService _storefrontService = storefrontService;
    private readonly ICouponService _couponService = couponService;
    private readonly IOrderService _orderService = orderService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IPaymentService _paymentService = paymentService;

    /// <summary>
    /// Obtém todos os dados da loja (Info, Menu, Categorias) em uma única requisição
    /// </summary>
    [HttpGet("storefront")]
    public async Task<ActionResult<ResponseDTO<MenuResponseDto>>> GetStorefrontData(string slug)
    {
        var response = await _storefrontService.GetStorefrontDataAsync(slug);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém informações detalhadas do tenant (loja)
    /// </summary>
    [HttpGet("info")]
    public async Task<ActionResult<ResponseDTO<TenantBusinessResponseDto?>>> GetTenantInfo(string slug)
    {
        var response = await _tenantService.GetTenantBusinessInfoBySlugAsync(slug);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém o menu completo (produtos ativos) para uma loja específica (slug)
    /// </summary>
    [HttpGet("menu")]
    public async Task<ActionResult<ResponseDTO<MenuResponseDto>>> GetMenu(string slug)
    {
        var response = await _productService.GetProductsForMenuBySlugAsync(slug);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém as categorias ativas para uma loja específica (slug)
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CategoryResponseDto>>>> GetCategories(string slug)
    {
        var response = await _categoryService.GetActiveCategoriesBySlugAsync(slug);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém produtos de uma categoria específica para uma loja (slug)
    /// </summary>
    [HttpGet("products/by-category/{categoryId}")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<ProductDto>>>> GetProductsByCategory(string slug, Guid categoryId)
    {
        var response = await _productService.GetProductsByCategoryAndSlugAsync(categoryId, slug);
        return BuildResponse(response);
    }

    /// <summary>
    /// Obtém detalhes de um produto específico para uma loja (slug)
    /// </summary>
    [HttpGet("products/{id}")]
    public async Task<ActionResult<ResponseDTO<ProductDto?>>> GetProduct(string slug, Guid id)
    {
        var response = await _productService.GetProductByIdAndSlugAsync(id, slug);
        return BuildResponse(response);
    }

    #region ORDERS
    /// <summary>
    /// Cria um novo pedido público para uma loja específica (slug)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    [HttpPost("orders")]
    public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> CreateOrder([FromBody]CreatePublicOrderRequestDto request, [FromRoute] string slug)
    {
        var serviceResponse = await _orderService.CreatePublicOrderAsync(request, slug);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Busca um pedido público por ID para uma loja específica (slug)
    /// </summary>
    /// <param name="slug"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("orders/{id}")]
    public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> GetOrder(string slug, Guid id)
    {
        var serviceResponse = await _orderService.GetPublicOrderByIdAsync(slug, id);
        return BuildResponse(serviceResponse);
    }

    [HttpGet("orders/customer/{customerId}")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<OrderResponseDto>>>> GetOrdersByCustomerId(string slug, string customerId)
    {
        var serviceResponse = await _orderService.GetPublicOrdersByCustomerIdAsync(slug, Guid.Parse(customerId));
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Cancela um pedido público (apenas se pendente)
    /// O cliente precisa estar logado para cancelar o seus pedidos
    /// </summary>
    [HttpPut("orders/{id}/cancel")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> CancelOrder(Guid id, [FromBody] CancelOrderRequestDto request)
    {
        // TODO: Validar se pedido pertence ao tenant do slug e ao customer (se auth disponível)
        var serviceResponse = await _orderService.CancelOrderAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualiza o método de pagamento de um pedido público (apenas se pendente)
    /// </summary>
    [HttpPut("orders/{id}/payment")]
    public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> UpdatePaymentMethod(string slug, Guid id, [FromBody] UpdateOrderPaymentRequestDto request)
    {
        var serviceResponse = await _orderService.UpdateOrderPaymentMethodAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Atualiza o tipo de entrega de um pedido público (apenas se pendente)
    /// </summary>
    [HttpPut("orders/{id}/delivery-type")]
    public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> UpdateDeliveryType(string slug, Guid id, [FromBody] UpdateOrderDeliveryTypeRequestDto request)
    {
        var serviceResponse = await _orderService.UpdateOrderDeliveryTypeAsync(id, request);
        return BuildResponse(serviceResponse);
    }

    /// <summary>
    /// Gera um pagamento PIX para um pedido público
    /// </summary>
    [HttpPost("orders/{id}/pix")]
    public async Task<ActionResult<ResponseDTO<PixResponseDto>>> GeneratePixPayment(string slug, Guid id)
    {
        // 1. Validar Tenant
        var tenantResponse = await _tenantService.GetTenantBusinessInfoBySlugAsync(slug);
        if (!tenantResponse.Succeeded || tenantResponse.Data == null)
            return NotFound(StaticResponseBuilder<PixResponseDto>.BuildError("Restaurante não encontrado"));
        
        // 2. Validar Pedido
        var orderResponse = await _orderService.GetPublicOrderByIdAsync(slug, id);
        if (!orderResponse.Succeeded || orderResponse.Data == null)
            return NotFound(StaticResponseBuilder<PixResponseDto>.BuildError("Pedido não encontrado"));
            
        var order = orderResponse.Data;
        
        // 3. Gerar PIX
        var pixRequest = new PixRequestDto
        {
            OrderId = id,
            Amount = order.Total,
            Description = $"Pedido #{order.Id}"
        };

        var serviceResponse = await _paymentService.GeneratePixAsync(pixRequest, tenantResponse.Data.Id);
        return BuildResponse(serviceResponse);
    }

    #endregion ORDERS
    #region LOGIN
    [HttpGet("customer/{phoneNumber}")]
    //inserir possíveis retorno
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 404)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 500)]
    public async Task<ActionResult<ResponseDTO<CustomerResponseDto>>> GetCustomer(string slug, string phoneNumber)
    {
        var serviceResponse = await _customerService.GetPublicCustomer(slug, phoneNumber);
        return BuildResponse(serviceResponse);
    }
    [HttpPost("customer/create")]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 404)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 500)]
    public async Task<ActionResult<ResponseDTO<CustomerResponseDto>>> CreateCustomer([FromBody] CreateCustomerRequestDto request, [FromRoute] string slug)
    {
        var serviceResponse = await _customerService.CreatePublicCustomerAsync(request, slug);
        return BuildResponse(serviceResponse);
    }

    [HttpPut("customer/update")]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 404)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<CustomerResponseDto>), 500)]
    public async Task<ActionResult<ResponseDTO<CustomerResponseDto>>> UpdateCustomer([FromBody] UpdateCustomerRequestDto request, [FromRoute] string slug)
    {
        var serviceResponse = await _customerService.UpdatePublicCustomerAsync(request, slug);
        return BuildResponse(serviceResponse);
    }

    #endregion LOGIN
    #region COUPONS
    [HttpPost("coupons/validate")]
    [ProducesResponseType(typeof(ResponseDTO<CouponDto>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<CouponDto>), 404)]
    [ProducesResponseType(typeof(ResponseDTO<CouponDto>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<CouponDto>), 500)]
    public async Task<ActionResult<ResponseDTO<CouponDto>>> ValidateCoupon([FromBody] ValidateCouponRequestDto request, [FromRoute] string slug)
    {
        var serviceReponse = await _couponService.ValidateCouponBySlugAsync(slug, request.Code, request.OrderValue);
        return BuildResponse(serviceReponse);
    }


    #endregion

}
