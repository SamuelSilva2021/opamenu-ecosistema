using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para AccessControlDbContext
/// </summary>
public class AccessControlRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public AccessControlRepository(AccessControlDbContext context) : base(context)
    {
    }
}
