using BotFramework;
using BotFramework.Attributes;
using BotFramework.Setup;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace MsgDumpBot
{
    class EventHandler:BotEventHandler
    {
        [Command("msgdump", CommandParseMode.Both)]
        public async Task MsgDump()
        {
            var msg = RawUpdate.Message;
            if(msg.ReplyToMessage != null)
            {
                string jmsg = JsonConvert.SerializeObject(msg, Formatting.Indented);
                await Bot.SendTextMessageAsync(Chat.Id, $"<code>{jmsg}</code>", ParseMode.Html);
            }
        }
    }
}
