using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.DTOs.Printing;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;
using OpaMenu.Desktop.Services.Implementation.Printing;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public sealed class PrintJobProcessor : IPrintJobProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AppDbContext _dbContext;
    private readonly IPrinterConfigurationService _printerConfigurationService;
    private readonly ILogger<PrintJobProcessor> _logger;
    private readonly EscPosRenderer _renderer = new();

    public PrintJobProcessor(
        AppDbContext dbContext,
        IPrinterConfigurationService printerConfigurationService,
        ILogger<PrintJobProcessor> logger)
    {
        _dbContext = dbContext;
        _printerConfigurationService = printerConfigurationService;
        _logger = logger;
    }

    public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await _dbContext.PrintJobs
            .Where(j => j.Status == EPrintJobStatus.Pending)
            .OrderBy(j => j.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (jobs.Count == 0)
            return;

        foreach (var job in jobs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessOneAsync(job.Id, cancellationToken);
        }
    }

    private async Task ProcessOneAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var job = await _dbContext.PrintJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job == null)
            return;

        if (job.Status != EPrintJobStatus.Pending)
            return;

        job.Status = EPrintJobStatus.Processing;
        job.LastAttemptAt = DateTime.UtcNow;
        job.Attempts += 1;
        job.LastError = null;
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var mapping = await _printerConfigurationService.GetMappingAsync(job.Destination, cancellationToken);
            if (mapping == null)
                throw new InvalidOperationException($"Nenhuma impressora configurada para o destino {job.Destination}.");

            var bytes = Render(job.PayloadType, job.PayloadJson, mapping);
            var transport = CreateTransport(mapping.ConnectionType);
            await transport.SendAsync(mapping, bytes, cancellationToken);

            job.Status = EPrintJobStatus.Printed;
            job.LastError = null;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao imprimir job {PrintJobId} ({Destination}/{PayloadType})", job.Id, job.Destination, job.PayloadType);
            job.Status = EPrintJobStatus.Error;
            job.LastError = ex.Message;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private byte[] Render(string payloadType, string payloadJson, PrinterMapping mapping)
    {
        if (string.Equals(payloadType, "TabBill", StringComparison.OrdinalIgnoreCase))
        {
            var payload = JsonSerializer.Deserialize<TabBillPrintPayload>(payloadJson, JsonOptions);
            if (payload == null)
                throw new InvalidOperationException("Payload inválido para TabBill.");

            return _renderer.RenderTabBill(payload, mapping);
        }

        throw new InvalidOperationException($"PayloadType não suportado: {payloadType}");
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

