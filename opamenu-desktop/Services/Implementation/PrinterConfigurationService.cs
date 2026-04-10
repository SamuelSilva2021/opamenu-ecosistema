using Microsoft.EntityFrameworkCore;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Entities;
using OpaMenu.Desktop.Models.Printing;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public sealed class PrinterConfigurationService : IPrinterConfigurationService
{
    private readonly AppDbContext _dbContext;

    public PrinterConfigurationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IReadOnlyList<string>> GetInstalledWindowsPrintersAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                using var server = new LocalPrintServer();
                return server
                    .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
                    .Select(q => q.Name)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Order(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }, cancellationToken);
    }

    public async Task<PrinterMapping?> GetMappingAsync(EPrintDestination destination, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.PrinterMappings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Destination == destination, cancellationToken);

        return entity == null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<PrinterMapping>> GetMappingsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.PrinterMappings
            .AsNoTracking()
            .OrderBy(x => x.Destination)
            .ToListAsync(cancellationToken);

        return entities.Select(Map).ToList();
    }

    public async Task UpsertMappingAsync(PrinterMapping mapping, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.PrinterMappings
            .FirstOrDefaultAsync(x => x.Destination == mapping.Destination, cancellationToken);

        if (entity == null)
        {
            entity = new PrinterMappingEntity
            {
                Destination = mapping.Destination
            };
            _dbContext.PrinterMappings.Add(entity);
        }

        entity.ConnectionType = mapping.ConnectionType;
        entity.PaperSize = mapping.PaperSize;
        entity.Profile = mapping.Profile;
        entity.WindowsPrinterName = string.IsNullOrWhiteSpace(mapping.WindowsPrinterName) ? null : mapping.WindowsPrinterName.Trim();
        entity.IpAddress = string.IsNullOrWhiteSpace(mapping.IpAddress) ? null : mapping.IpAddress.Trim();
        entity.Port = mapping.Port;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static PrinterMapping Map(PrinterMappingEntity entity)
    {
        return new PrinterMapping(
            entity.Destination,
            entity.ConnectionType,
            entity.PaperSize,
            entity.Profile,
            entity.WindowsPrinterName,
            entity.IpAddress,
            entity.Port
        );
    }
}

