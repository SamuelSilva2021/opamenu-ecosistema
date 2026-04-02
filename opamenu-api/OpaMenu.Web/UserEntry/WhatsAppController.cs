//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using OpaMenu.Application.Services.Interfaces.Opamenu;
//using OpaMenu.Infrastructure.Filters;
//using OpaMenu.Infrastructure.Anotations;
//using OpaMenu.Web.UserEntry;
//using System.Text.Json;

//namespace OpaMenu.Web.UserEntry;

//[ApiController]
//[Route("api/[controller]")]
//[Authorize]
//public class WhatsAppController(IWhatsAppService whatsAppService, ILogger<WhatsAppController> logger) : BaseController
//{
//    private readonly IWhatsAppService _whatsAppService = whatsAppService;
//    private readonly ILogger<WhatsAppController> _logger = logger;

//    [HttpPost("webhook/{tenantId}")]
//    [AllowAnonymous]
//    public async Task<IActionResult> HandleWebhook(Guid tenantId, [FromBody] JsonElement payload)
//    {
//        try
//        {
//            _logger.LogInformation("Recebido webhook WhatsApp para Tenant {TenantId}", tenantId);

//            if (payload.TryGetProperty("data", out var data))
//            {
//                var phoneNumber = data.GetProperty("key").GetProperty("remoteJid").GetString()?.Split('@')[0];
//                var message = data.GetProperty("message").GetProperty("conversation").GetString();

//                if (!string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(message))
//                {
//                    await _whatsAppService.ProcessIncomingMessageAsync(tenantId, phoneNumber, message);
//                }
//            }

//            return Ok();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Erro ao processar webhook WhatsApp");
//            return BadRequest();
//        }
//    }

//    [HttpGet("status")]
//    public async Task<IActionResult> GetStatus()
//    {
//        var tenantId = GetTenantId();
//        if (tenantId == null) return Unauthorized();

//        var isConnected = await _whatsAppService.IsInstanceConnectedAsync(tenantId.Value);
//        return Ok(new { connected = isConnected });
//    }

//    [HttpPost("test-message")]
//    public async Task<IActionResult> SendTestMessage([FromBody] TestMessageRequest request)
//    {
//        var tenantId = GetTenantId();
//        if (tenantId == null) return Unauthorized();

//        var success = await _whatsAppService.SendTextMessageAsync(tenantId.Value, request.PhoneNumber, request.Message);
//        return success ? Ok() : BadRequest("Falha ao enviar mensagem");
//    }

//    private Guid? GetTenantId()
//    {
//        return Guid.Parse(User.FindFirst("tenant_id")?.Value ?? Guid.Empty.ToString());
//    }
//}

//public record TestMessageRequest(string PhoneNumber, string Message);
