using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.CashRegister;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface ICashRegisterService
{
    Task<ResponseDTO<CashShiftResponseDto>> GetActiveShiftAsync();
    Task<ResponseDTO<CashShiftSummaryResponseDto>> GetActiveShiftSummaryAsync();
    Task<ResponseDTO<CashShiftResponseDto>> OpenShiftAsync(OpenCashShiftRequestDto request);
    Task<ResponseDTO<CashShiftResponseDto>> CloseShiftAsync(CloseCashShiftRequestDto request);
    Task<ResponseDTO<CashShiftCloseSummaryResponseDto>> CloseShiftWithSummaryAsync(CloseCashShiftRequestDto request);
    Task<ResponseDTO<CashMovementResponseDto>> AddMovementAsync(AddCashMovementRequestDto request);
    Task<ResponseDTO<IEnumerable<CashShiftResponseDto>>> GetShiftHistoryAsync(int count = 10);
    Task<ResponseDTO<CashRegisterReportDto>> GetReportAsync(DateTime startDate, DateTime endDate);
}
