using Microsoft.EntityFrameworkCore;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.Entities;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;
using OpaMenu.Desktop.Services.Implementation.Printing;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public sealed class PrintService : IPrintService
{
    private readonly AppDbContext _dbContext;
    private readonly EscPosRenderer _renderer;

    public PrintService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _renderer = new EscPosRenderer();
    }

    public async Task<Guid> EnqueueAsync(PrintJobCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PayloadType))
            throw new InvalidOperationException("PayloadType é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.PayloadJson))
            throw new InvalidOperationException("PayloadJson é obrigatório.");

        var entity = new PrintJobEntity
        {
            Destination = request.Destination,
            PayloadType = request.PayloadType.Trim(),
            PayloadJson = request.PayloadJson,
            Status = EPrintJobStatus.Pending
        };

        _dbContext.PrintJobs.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> PrintTestAsync(PrinterMapping mapping, CancellationToken cancellationToken = default)
    {
        var bytes = _renderer.RenderTest(mapping);
        var transport = CreateTransport(mapping.ConnectionType);
        await transport.SendAsync(mapping, bytes, cancellationToken);
        return true;
    }

    private static IPrintTransport CreateTransport(EPrinterConnectionType type)
    {
        return type switch
        {
            EPrinterConnectionType.Tcp9100 => new Tcp9100PrintTransport(),
            EPrinterConnectionType.WindowsSpoolerRaw => new WindowsRawPrintTransport(),
            _ => new Tcp9100PrintTransport()
        };
    }
}

