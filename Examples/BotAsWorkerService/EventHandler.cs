using System;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Attributes;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAsWorkerService
{
    public class EventHandler:BotEventHandler
    {
        [Command("start")]
        public async Task Start() => await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");

        [ParametrizedCommand("me", CommandParseMode.Both)]
        public async Task Me(MeParam me)
        {
            var user = From.FirstName;
            user = !string.IsNullOrEmpty(From.LastName) ? user + $" {From.LastName}" : user;
            await Bot.SendTextMessageAsync(Chat, $"<code>{user} says: {me.Text}</code>", ParseMode.Html);
        }
    }

    public class MeParam
    {
        public string Text;
    }
    public class MeParser:IRawParameterParser<MeParam>
    {
        public MeParam DefaultInstance() { return new MeParam();}

        public bool TryGetValueByRawUpdate(Update update, out MeParam result)
        {
            if(update.Type != UpdateType.Message || update.Message.Type != MessageType.Text)
            {
                result = null;
                return false;
            }
            var me = new MeParam();
            me.Text = update.Message.Text.Replace("/me ", string.Empty);

            result = me;
            return true;
        }
    }
}
