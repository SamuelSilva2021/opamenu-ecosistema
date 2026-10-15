using System;
using System.Threading;
using System.Threading.Tasks;
using Opamenu.Desktop.Models.Printing;

namespace Opamenu.Desktop.Services.Interfaces;

public interface IPrintService
{
    Task<Guid> EnqueueAsync(PrintJobCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> PrintTestAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
}

