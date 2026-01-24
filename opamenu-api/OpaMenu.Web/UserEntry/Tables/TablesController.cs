using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Table;
using OpaMenu.Domain.DTOs.Order;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Application.Services.Interfaces;

namespace OpaMenu.Web.UserEntry.Tables
{
    [Route("api/tables")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class TablesController(ITableService tableService, IOrderService orderService) : BaseController
    {
        private readonly ITableService _tableService = tableService;
        private readonly IOrderService _orderService = orderService;

        /// <summary>
        /// Obtém lista paginada de mesas
        /// </summary>
        [HttpGet]
        [MapPermission(MODULE_TABLE, OPERATION_SELECT)]
        public async Task<ActionResult<PagedResponseDTO<TableResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _tableService.GetPagedAsync(pageNumber, pageSize);
            return BuildResponse(result);
        }

        /// <summary>
        /// Obtém uma mesa pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [MapPermission(MODULE_TABLE, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<TableResponseDto>>> GetById(Guid id)
        {
            var result = await _tableService.GetByIdAsync(id);
            return BuildResponse(result);
        }
        
        /// <summary>
        /// Obtém o pedido ativo da mesa
        /// </summary>
        [HttpGet("{id}/order")]
        [MapPermission(MODULE_TABLE, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<OrderResponseDto?>>> GetActiveOrder(Guid id)
        {
            var result = await _orderService.GetActiveOrderByTableIdAsync(id);
            return BuildResponse(result);
        }

        /// <summary>
        /// Cria uma nova mesa
        /// </summary>
        [HttpPost]
        [MapPermission(MODULE_TABLE, OPERATION_INSERT)]
        public async Task<ActionResult<ResponseDTO<TableResponseDto>>> Create([FromBody] CreateTableRequestDto dto)
        {
            var result = await _tableService.CreateAsync(dto);
            return BuildResponse(result);
        }
        
        /// <summary>
        /// Fecha a conta da mesa
        /// </summary>
        [HttpPost("{id}/close")]
        [MapPermission(MODULE_TABLE, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> CloseAccount(Guid id)
        {
            var result = await _orderService.CloseTableAccountAsync(id);
            return BuildResponse(result);
        }

        /// <summary>
        /// Atualiza uma mesa existente
        /// </summary>
        [HttpPut("{id}")]
        [MapPermission(MODULE_TABLE, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<TableResponseDto>>> Update(Guid id, [FromBody] UpdateTableRequestDto dto)
        {
            var result = await _tableService.UpdateAsync(id, dto);
            return BuildResponse(result);
        }

        /// <summary>
        /// Remove uma mesa
        /// </summary>
        [HttpDelete("{id}")]
        [MapPermission(MODULE_TABLE, OPERATION_DELETE)]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
        {
            var result = await _tableService.DeleteAsync(id);
            return BuildResponse(result);
        }

        /// <summary>
        /// Gera o QR Code para uma mesa
        /// </summary>
        [HttpPost("{id}/qrcode")]
        [MapPermission(MODULE_TABLE, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<string>>> GenerateQrCode(Guid id)
        {
            var result = await _tableService.GenerateQrCodeAsync(id);
            return BuildResponse(result);
        }
    }
}
