using System.Threading;
using System.Threading.Tasks;

namespace BotFramework.Abstractions.UpdateProvider;

public interface IUpdateProvider
{
    Task StartAsync(CancellationToken token);
    Task StopAsync(CancellationToken token);
}
