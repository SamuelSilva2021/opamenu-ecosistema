using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Web.UserEntry;

/// <summary>
/// Controller para gerenciamento de pagamentos seguindo princípios SOLID e Clean Architecture
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger) : BaseController
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentsController> _logger = logger;

    /// <summary>
    /// Obtém lista de pagamentos
    /// </summary>
    /// <returns>Lista de pagamentos</returns>
    [HttpGet]
    [MapPermission(MODULE_PAYMENT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponseDto>>>> GetPayments()
    {
        try
        {
            var payments = await _paymentService.GetPaymentsAsync();
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.SuccessResponse(payments, "Pagamentos obtidos com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pagamentos");
            return StatusCode(500, ApiResponse<IEnumerable<PaymentResponseDto>>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Gera um pagamento PIX
    /// </summary>
    /// <param name="request">Dados para geração do PIX</param>
    /// <returns>Dados do PIX gerado</returns>
    [HttpPost("pix/generate")]
    [MapPermission(MODULE_PAYMENT, OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<PixResponseDto>>> GeneratePixPayment([FromBody] PixRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<PixResponseDto>.ErrorResponse("Dados inválidos"));
            }

            var result = await _paymentService.GeneratePixAsync(request);
            if (!result.Succeeded)
            {
                var message = result.Errors?.FirstOrDefault()?.Message ?? result.Exception?.Message ?? "Erro ao gerar PIX";
                return BadRequest(ApiResponse<PixResponseDto>.ErrorResponse(message));
            }
            return Ok(ApiResponse<PixResponseDto>.SuccessResponse(result.Data!, "PIX gerado com sucesso"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para geração de PIX");
            return BadRequest(ApiResponse<PixResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar pagamento PIX");
            return StatusCode(500, ApiResponse<PixResponseDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Endpoint genérico para Webhooks de pagamento
    /// </summary>
    [HttpPost("webhook/{tenantId}/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleWebhook(Guid tenantId, string provider)
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Signature"].ToString() ?? Request.Headers["X-Event-Id"].ToString() ?? string.Empty;

            _logger.LogInformation("Recebido webhook para Tenant {TenantId}, Provider {Provider}. Payload size: {Size}", tenantId, provider, payload.Length);

            var result = await _paymentService.ProcessWebhookAsync(tenantId, provider, payload, signature);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                var message = result.Errors?.FirstOrDefault()?.Message ?? result.Exception?.Message ?? "Erro ao processar webhook";
                _logger.LogWarning("Falha ao processar webhook: {Message}", message);
                return BadRequest(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no webhook");
            return StatusCode(500, "Erro interno");
        }
    }
    /// <param name="paymentId">ID do pagamento</param>
    /// <returns>Status do pagamento</returns>
    [HttpGet("{paymentId}/status")]
    [MapPermission(MODULE_PAYMENT, OPERATION_SELECT)]
    public async Task<ActionResult<ApiResponse<PaymentStatusDto>>> GetPaymentStatus(Guid paymentId)
    {
        try
        {
            var result = await _paymentService.GetPaymentStatusAsync(paymentId);
            return Ok(ApiResponse<PaymentStatusDto>.SuccessResponse(result, "Status obtido com sucesso"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "ID de pagamento inválido: {PaymentId}", paymentId);
            return BadRequest(ApiResponse<PaymentStatusDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar status do pagamento {PaymentId}", paymentId);
            return StatusCode(500, ApiResponse<PaymentStatusDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

}
