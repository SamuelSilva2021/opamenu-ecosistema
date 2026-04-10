using System.Threading;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Interfaces;

public interface IPrintJobProcessor
{
    Task ProcessPendingAsync(CancellationToken cancellationToken = default);
}

