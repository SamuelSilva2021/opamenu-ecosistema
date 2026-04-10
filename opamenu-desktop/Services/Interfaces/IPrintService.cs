using System;
using System.Threading;
using System.Threading.Tasks;
using OpaMenu.Desktop.Models.Printing;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface IPrintService
{
    Task<Guid> EnqueueAsync(PrintJobCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> PrintTestAsync(PrinterMapping mapping, CancellationToken cancellationToken = default);
}

