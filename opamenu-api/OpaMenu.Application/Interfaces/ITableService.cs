using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Table;

namespace OpaMenu.Application.Interfaces;

public interface ITableService
{
    Task<PagedResponseDTO<TableResponseDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<ResponseDTO<TableResponseDto>> GetByIdAsync(Guid id);
    Task<ResponseDTO<TableResponseDto>> CreateAsync(CreateTableRequestDto dto);
    Task<ResponseDTO<TableResponseDto>> UpdateAsync(Guid id, UpdateTableRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<string>> GenerateQrCodeAsync(Guid id);
}
