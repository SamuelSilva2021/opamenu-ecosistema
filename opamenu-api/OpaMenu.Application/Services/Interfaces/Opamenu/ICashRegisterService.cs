using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.CashRegister;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ICashRegisterService
{
    Task<ResponseDTO<CashShiftResponseDto>> GetActiveShiftAsync();
    Task<ResponseDTO<CashShiftResponseDto>> OpenShiftAsync(OpenCashShiftRequestDto request);
    Task<ResponseDTO<CashShiftResponseDto>> CloseShiftAsync(CloseCashShiftRequestDto request);
    Task<ResponseDTO<CashMovementResponseDto>> AddMovementAsync(AddCashMovementRequestDto request);
    Task<ResponseDTO<IEnumerable<CashShiftResponseDto>>> GetShiftHistoryAsync(int count = 10);
}
