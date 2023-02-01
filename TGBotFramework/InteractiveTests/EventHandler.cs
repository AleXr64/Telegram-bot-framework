using BotFramework;
using BotFramework.Attributes;
using Telegram.Bot;
using System.Reflection;
namespace InteractiveTests;

public class EventHandler : BotEventHandler
{
    bool? AnyUpdateOk = null;

    [TextMessage("OK")]
    public async Task TestOk()
    {

    }
    [Update]
    public async Task AnyUpdate()
    {
        await Bot.SendTextMessageAsync(Chat.Id, MethodBase.GetCurrentMethod().Name);
    }

    [Message]
    public async Task AnyMessage()
    {
        await Bot.SendTextMessageAsync(Chat.Id, MethodBase.GetCurrentMethod().Name);
    }
}
