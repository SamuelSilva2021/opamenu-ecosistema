using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.Table;

namespace OpaMenu.Application.Interfaces;

public interface ITableService
{
    Task<PagedResponseDTO<TableResponseDto>> GetPagedAsync(int pageNumber, int pageSize);
    Task<ResponseDTO<TableResponseDto>> GetByIdAsync(int id);
    Task<ResponseDTO<TableResponseDto>> CreateAsync(CreateTableRequestDto dto);
    Task<ResponseDTO<TableResponseDto>> UpdateAsync(int id, UpdateTableRequestDto dto);
    Task<ResponseDTO<bool>> DeleteAsync(int id);
    Task<ResponseDTO<string>> GenerateQrCodeAsync(int id);
}
