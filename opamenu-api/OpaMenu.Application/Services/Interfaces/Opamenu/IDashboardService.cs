using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface IDashboardService
{
    Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync();
}
