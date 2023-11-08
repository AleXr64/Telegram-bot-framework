using System.Collections.Generic;
using System.Threading;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public interface IUpdateSource
{
    public ManualResetEvent ShouldProcess { get; }

    public void Push(Update update);
    public Update Pull();
}
