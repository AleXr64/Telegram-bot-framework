using System.Threading;
using System.Threading.Tasks;

namespace BotFramework.Abstractions;

public interface IUpdateProvider
{
    Task StartAsync(CancellationToken token);
    Task StopAsync(CancellationToken token);
}
