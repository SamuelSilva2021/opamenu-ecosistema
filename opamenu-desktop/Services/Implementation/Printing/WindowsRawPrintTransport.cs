using OpaMenu.Desktop.Models.Printing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation.Printing;

internal sealed class WindowsRawPrintTransport : IPrintTransport
{
    public Task SendAsync(PrinterMapping mapping, byte[] bytes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mapping.WindowsPrinterName))
            throw new InvalidOperationException("Impressora do Windows não configurada.");

        cancellationToken.ThrowIfCancellationRequested();

        var ok = WindowsRawPrinter.SendBytesToPrinter(mapping.WindowsPrinterName.Trim(), bytes);
        if (!ok)
            throw new InvalidOperationException("Falha ao enviar dados para o spooler do Windows.");

        return Task.CompletedTask;
    }
}

