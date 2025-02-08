using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Utils
{
    public static class BotExt
    {
        public static Task<Message> SendHtmlStringAsync(this ITelegramBotClient bot, Chat chat, HtmlString message, int? replyTo = null)
        {
            if(replyTo == null)
            {
                return bot.SendMessage(chat, message.ToString(), parseMode: ParseMode.Html);
            }
            var replyParams = new ReplyParameters() { ChatId = chat.Id, MessageId = replyTo.Value };
            return bot.SendMessage(chat, message.ToString(), parseMode: ParseMode.Html, replyParams);
        }
    }
}
