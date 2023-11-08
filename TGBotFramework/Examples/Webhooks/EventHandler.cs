using BotFramework;
using BotFramework.Attributes;
using BotFramework.Utils;
using Newtonsoft.Json;

namespace Webhooks;

public class EventHandler : BotEventHandler
{
    [Command("dump")]
    public async Task HandleDump()
    {
        var message = RawUpdate.Message!.ReplyToMessage ?? RawUpdate.Message;
        var text = HtmlString.Empty
           .CodeWithStyle("language-json", JsonConvert.SerializeObject(message, Formatting.Indented));

        await Bot.SendHtmlStringAsync(Chat, text, replyTo: message.MessageId);
    }
}
