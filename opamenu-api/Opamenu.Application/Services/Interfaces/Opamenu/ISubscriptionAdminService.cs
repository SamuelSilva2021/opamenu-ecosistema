using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.MultiTenant;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface ISubscriptionAdminService
{
    Task<(ResponseDTO<string> Body, int StatusCode)> ActivatePlanAsync(Guid planId, Guid? tenantId = null);
    Task<SubscriptionDto> GetByTenantAsync(Guid tenantId);
}

