using System;
using Telegram.Bot;

namespace BotFramework.Abstractions
{
    public interface IBotInstance
    {
        string UserName { get; }

        ITelegramBotClient BotClient { get; }

        IBotPlugin GetPlugin<TPlugin>() where TPlugin: IBotPlugin => throw new NotImplementedException("Not implemented yet!");
    }
}
