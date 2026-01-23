using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Payments;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;

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
    /// Processa um pagamento
    /// </summary>
    /// <param name="request">Dados do pagamento</param>
    /// <returns>Resultado do processamento</returns>
    [HttpPost("process")]
    [MapPermission(MODULE_PAYMENT, OPERATION_INSERT)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> ProcessPayment([FromBody] PaymentRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<PaymentResponseDto>.ErrorResponse("Dados inválidos"));
            }

            var result = await _paymentService.ProcessPaymentAsync(request);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "Pagamento processado com sucesso"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para processamento de pagamento");
            return BadRequest(ApiResponse<PaymentResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pagamento");
            return StatusCode(500, ApiResponse<PaymentResponseDto>.ErrorResponse("Erro interno do servidor"));
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

            var result = await _paymentService.GeneratePixPaymentAsync(request);
            return Ok(ApiResponse<PixResponseDto>.SuccessResponse(result, "PIX gerado com sucesso"));
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
    /// Obtém o status de um pagamento
    /// </summary>
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

    /// <summary>
    /// Processa estorno de um pagamento
    /// </summary>
    /// <param name="paymentId">ID do pagamento</param>
    /// <param name="request">Dados do estorno</param>
    /// <returns>Resultado do estorno</returns>
    [HttpPost("{paymentId}/refund")]
    [MapPermission(MODULE_PAYMENT, OPERATION_REVERSAL)]
    public async Task<ActionResult<ApiResponse<RefundResponseDto>>> RefundPayment(Guid paymentId, [FromBody] RefundRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RefundResponseDto>.ErrorResponse("Dados inválidos"));
            }

            // Definir o PaymentId no request
            request.PaymentId = paymentId;
            
            var result = await _paymentService.RefundPaymentAsync(request);
            return Ok(ApiResponse<RefundResponseDto>.SuccessResponse(result, "Estorno processado com sucesso"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para estorno do pagamento {PaymentId}", paymentId);
            return BadRequest(ApiResponse<RefundResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar estorno do pagamento {PaymentId}", paymentId);
            return StatusCode(500, ApiResponse<RefundResponseDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

    // TODO: Implement webhook handlers
    [HttpPost("webhook/stripe")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook([FromBody] string payload)
    {
        // TODO: Implement Stripe webhook handler
        return Ok();
    }

    [HttpPost("webhook/pagseguro")]
    [AllowAnonymous]
    public async Task<IActionResult> PagSeguroWebhook([FromBody] object notification)
    {
        // TODO: Implement PagSeguro webhook handler
        return Ok();
    }
}
