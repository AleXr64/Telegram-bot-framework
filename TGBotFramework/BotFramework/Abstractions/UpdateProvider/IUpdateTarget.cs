using Telegram.Bot.Types;

namespace BotFramework.Abstractions.UpdateProvider;

public interface IUpdateTarget
{
    public void Push(Update update);
}
