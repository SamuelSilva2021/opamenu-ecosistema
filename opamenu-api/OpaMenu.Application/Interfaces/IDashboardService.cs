using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Application.Interfaces;

public interface IDashboardService
{
    Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync();
}
