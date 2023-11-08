#nullable enable

using System.Collections.Concurrent;
using System.Threading;
using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework;

public class ConcurrentQueueUpdateSource : IUpdateSource
{
    private readonly ConcurrentQueue<Update> _queue = new();

    public ManualResetEvent ShouldProcess { get; } = new(false);

    public void Push(Update update)
    {
        _queue.Enqueue(update);
        ShouldProcess.Set();
    }

    public Update? Pull()
    {
        return _queue.TryDequeue(out var update) ? update : null;
    }
}
