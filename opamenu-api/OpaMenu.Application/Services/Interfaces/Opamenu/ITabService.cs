using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Tab;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

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
