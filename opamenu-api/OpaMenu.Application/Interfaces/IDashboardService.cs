using OpaMenu.Application.DTOs;
using OpaMenu.Application.DTOs.Dashboard;

namespace OpaMenu.Application.Interfaces;

public interface IDashboardService
{
    Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync();
}
