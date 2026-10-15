using System.Threading;
using System.Threading.Tasks;
using Opamenu.Desktop.Models.Printing;

namespace Opamenu.Desktop.Services.Implementation.Printing;

internal interface IPrintTransport
{
    Task SendAsync(PrinterMapping mapping, byte[] bytes, CancellationToken cancellationToken);
}

