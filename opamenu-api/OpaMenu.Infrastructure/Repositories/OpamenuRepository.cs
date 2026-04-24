using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para OpamenuDbContext
/// </summary>
public class OpamenuRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public OpamenuRepository(OpamenuDbContext context) : base(context)
    {
    }
}
