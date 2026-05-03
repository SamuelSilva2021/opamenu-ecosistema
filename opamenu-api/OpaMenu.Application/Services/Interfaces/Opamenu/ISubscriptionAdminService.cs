using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.MultiTenant;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ISubscriptionAdminService
{
    Task<(ResponseDTO<string> Body, int StatusCode)> ActivatePlanAsync(Guid planId, Guid? tenantId = null);
    Task<SubscriptionDto> GetByTenantAsync(Guid tenantId);
}

