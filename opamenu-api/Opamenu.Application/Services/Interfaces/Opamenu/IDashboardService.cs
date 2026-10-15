using Opamenu.Application.DTOs.Dashboard;
using Opamenu.Commons.Api.DTOs;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface IDashboardService
{
    Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
}
