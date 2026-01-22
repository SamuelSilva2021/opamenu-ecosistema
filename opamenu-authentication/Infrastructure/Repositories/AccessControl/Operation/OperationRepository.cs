using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Operation
{
    public class OperationRepository(IDbContextProvider dbContextProvider, AccessControlDbContext context, ILogger<OperationRepository> logger) : 
        BaseRepository<OperationEntity>(dbContextProvider), IOperationRepository
    {
        private readonly ILogger<OperationRepository> _logger = logger;
        private readonly AccessControlDbContext _context = context;

        public async Task<bool> ExisteValue(string value) =>
            await _context.Operations.AnyAsync(op => op.Value == value);
    }
}

