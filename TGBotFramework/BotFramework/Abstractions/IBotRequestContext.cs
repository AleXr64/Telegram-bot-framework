using System.Collections.Generic;
using BotFramework.Abstractions.Storage;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IBotRequestContext
    {
        IBotInstance Instance { get; }

        Update Update { get; }

        IReadOnlyList<IBotRequestHandler> PossibleHandlers { get; }

        IBotUser BotUser { get; }
    }
}
