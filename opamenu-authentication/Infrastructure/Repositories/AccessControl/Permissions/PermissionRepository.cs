using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Permissions
{
    public class PermissionRepository(
        IDbContextProvider dbContextProvider,
        ILogger<PermissionRepository> logger
            ) : BaseRepository<PermissionEntity>(dbContextProvider), IPermissionRepository
    {
        private readonly ILogger<PermissionRepository> _logger = logger;
    }
}

