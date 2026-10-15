using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para AccessControlDbContext
/// </summary>
public class AccessControlRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public AccessControlRepository(AccessControlDbContext context) : base(context)
    {
    }
}
