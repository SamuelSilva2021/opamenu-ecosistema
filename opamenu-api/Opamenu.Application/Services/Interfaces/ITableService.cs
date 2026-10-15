using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Table;

namespace Opamenu.Application.Services.Interfaces;

public interface ITableService
{
    Task<PagedResponseDTO<TableResponseDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<ResponseDTO<TableResponseDto>> GetByIdAsync(Guid id);
    Task<PagedResponseDTO<TableFullResponseDto>> GetPagedWithTabsAsync(int pageNumber, int pageSize);
    Task<ResponseDTO<TableFullResponseDto>> GetByIdWithDetailsAsync(Guid id);
    Task<ResponseDTO<TableResponseDto>> CreateAsync(CreateTableRequestDto dto);
    Task<ResponseDTO<TableResponseDto>> UpdateAsync(Guid id, UpdateTableRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<string>> GenerateQrCodeAsync(Guid id);
    Task<ResponseDTO<bool>> UpdateLayoutsAsync(List<UpdateTableLayoutRequestDto> dtos);
}
