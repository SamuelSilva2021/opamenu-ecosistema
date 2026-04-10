using OpaMenu.Desktop.Models.Printing;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation.Printing;

internal sealed class Tcp9100PrintTransport : IPrintTransport
{
    public async Task SendAsync(PrinterMapping mapping, byte[] bytes, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mapping.IpAddress))
            throw new InvalidOperationException("IP da impressora não configurado.");

        var port = mapping.Port ?? 9100;
        using var client = new TcpClient();
        using var registration = cancellationToken.Register(() =>
        {
            try { client.Dispose(); } catch { }
        });

        await client.ConnectAsync(mapping.IpAddress.Trim(), port, cancellationToken);
        await using var stream = client.GetStream();
        await stream.WriteAsync(bytes, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}

