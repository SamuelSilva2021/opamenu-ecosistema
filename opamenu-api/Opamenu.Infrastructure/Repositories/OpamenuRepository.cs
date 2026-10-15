using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para OpamenuDbContext
/// </summary>
public class OpamenuRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public OpamenuRepository(OpamenuDbContext context) : base(context)
    {
    }
}
