using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs;
using Opamenu.Domain.DTOs.Tab;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface ITabService
{
    Task<ResponseDTO<IEnumerable<TabResponseDto>>> GetByTableIdAsync(Guid tableId, ETabStatus? status = null);
    Task<ResponseDTO<TabResponseDto>> GetByIdAsync(Guid tableId, Guid tabId);
    Task<ResponseDTO<TabResponseDto>> OpenAsync(Guid tableId, CreateTabRequestDto dto);
    Task<ResponseDTO<TabResponseDto>> CloseAsync(Guid tableId, Guid tabId);
    Task<ResponseDTO<TabResponseDto>> CheckoutAsync(Guid tableId, Guid tabId, TabCheckoutRequestDto dto);
    Task<ResponseDTO<TabResponseDto>> UpdateAsync(Guid tabId, UpdateTabRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid tabId);
    Task<ResponseDTO<IEnumerable<OrderItemResponseDto>>> GetItemsAsync(Guid tabId);
    Task<ResponseDTO<OrderResponseDto>> AddItemsAsync(Guid tabId, List<CreateOrderItemRequestDto> items);
}
