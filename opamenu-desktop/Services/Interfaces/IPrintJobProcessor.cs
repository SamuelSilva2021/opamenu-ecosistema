using System.Threading;
using System.Threading.Tasks;

namespace Opamenu.Desktop.Services.Interfaces;

public interface IPrintJobProcessor
{
    Task ProcessPendingAsync(CancellationToken cancellationToken = default);
}

