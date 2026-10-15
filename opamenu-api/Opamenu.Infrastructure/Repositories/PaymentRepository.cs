using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

public class PaymentRepository(OpamenuDbContext context) : OpamenuRepository<PaymentEntity>(context)
{
}
