using System;
using Telegram.Bot;

namespace BotFramework.Abstractions
{
    public interface IBotInstance
    {
        string UserName { get; }

        ITelegramBotClient BotClient { get; }
#if NETSTANDARD2_1
        IBotPlugin GetPlugin<TPlugin>() where TPlugin: IBotPlugin => throw new NotImplementedException("Not implemented yet!");
#endif
    }
}
