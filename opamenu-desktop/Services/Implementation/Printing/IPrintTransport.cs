using System.Threading;
using System.Threading.Tasks;
using OpaMenu.Desktop.Models.Printing;

namespace OpaMenu.Desktop.Services.Implementation.Printing;

internal interface IPrintTransport
{
    Task SendAsync(PrinterMapping mapping, byte[] bytes, CancellationToken cancellationToken);
}

