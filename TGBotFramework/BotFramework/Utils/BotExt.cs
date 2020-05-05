using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Utils
{
    public static class BotExt
    {
        public static Task<Message> SendHtmlStringAsync(this ITelegramBotClient bot, Chat chat, HtmlString message)
        {
            return bot.SendTextMessageAsync(chat, message.ToString(), ParseMode.Html);
        }
    }
}
