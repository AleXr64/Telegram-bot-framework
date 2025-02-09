using System.Text.Json;
using BotFramework;
using BotFramework.Attributes;
using BotFramework.Utils;

namespace Webhooks;

public class EventHandler : BotEventHandler
{
    private static readonly JsonSerializerOptions DumpOptions = new() { WriteIndented = true };

    [Command("dump")]
    public async Task HandleDump()
    {
        var message = RawUpdate.Message!.ReplyToMessage ?? RawUpdate.Message;
        var text = HtmlString.Empty
           .CodeWithStyle("language-json", JsonSerializer.Serialize(message, DumpOptions));

        await Bot.SendHtmlStringAsync(Chat, text, replyTo: message.MessageId);
    }
}
