using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Module
{
    public interface IModuleRepository : IBaseRepository<ModuleEntity>
    {
        Task<UserPermissionsDTO> GetModulesByUserAsync(Guid userId);
    }
}

