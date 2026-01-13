using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.AccessGroup
{
    /// <summary>
    /// Implementação específica do repositório para tipos de grupo
    /// </summary>
    public class GroupTypeRepository(
            IDbContextProvider dbContextProvider,
            IMemoryCache cache,
            ILogger<GroupTypeRepository> logger
        ) : BaseRepository<GroupTypeEntity>(dbContextProvider), IGroupTypeRepository
    {
        private readonly ILogger<GroupTypeRepository> _logger = logger;
        private readonly IMemoryCache _cache = cache;

    }
}

