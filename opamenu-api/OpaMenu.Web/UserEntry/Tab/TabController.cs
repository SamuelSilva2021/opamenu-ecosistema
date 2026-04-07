using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Tab;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Web.UserEntry.Tab
{
    [Route("api/tabs")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class TabController(ITabService tabService) : BaseController
    {
        private readonly ITabService _tabService = tabService;

        /// <summary>
        /// Atualiza uma comanda existente
        /// </summary>
        [HttpPut("{id}")]
        [MapPermission(MODULE_TABLE, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<TabResponseDto>>> Update(Guid id, [FromBody] UpdateTabRequestDto dto)
        {
            var result = await _tabService.UpdateAsync(id, dto);
            return BuildResponse(result);
        }

        /// <summary>
        /// Remove uma comanda
        /// </summary>
        [HttpDelete("{id}")]
        [MapPermission(MODULE_TABLE, OPERATION_DELETE)]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
        {
            var result = await _tabService.DeleteAsync(id);
            return BuildResponse(result);
        }

        /// <summary>
        /// Lista os itens vinculados ao pedido da comanda
        /// </summary>
        [HttpGet("{id}/items")]
        [MapPermission(MODULE_TABLE, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<OrderItemResponseDto>>>> GetItems(Guid id)
        {
            var result = await _tabService.GetItemsAsync(id);
            return BuildResponse(result);
        }

        /// <summary>
        /// Inclui itens no pedido da comanda
        /// </summary>
        [HttpPost("{id}/items")]
        [MapPermission(MODULE_TABLE, OPERATION_INSERT)]
        public async Task<ActionResult<ResponseDTO<OrderResponseDto>>> AddItems(Guid id, [FromBody] List<CreateOrderItemRequestDto> items)
        {
            var result = await _tabService.AddItemsAsync(id, items);
            return BuildResponse(result);
        }
    }
}
