using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class PaymentRepository(OpamenuDbContext context) : OpamenuRepository<PaymentEntity>(context)
{
}
