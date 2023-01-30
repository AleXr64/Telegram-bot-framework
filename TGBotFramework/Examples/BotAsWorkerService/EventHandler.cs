using BotFramework;
using BotFramework.Attributes;
using BotFramework.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAsWorkerService
{
    public class EventHandler:BotEventHandler
    {
        /* Answer on "/start" command from private messages
         * Priority:
         * You can change order of execution for handlers with Priority attribute.
         * Higher value means higher priority. Default is 0.
         * Return false to prevent execution handlers with less priority, or return true to continue
         */
        [Command("start", CommandParseMode.Both), Priority(10)]
        public async Task<bool> Start()
        {
            await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");
            return false;
        }

        //Answer on message with "ban" text
        [TextMessage("ban")]
        public async Task Ban() => await Bot.SendTextMessageAsync(Chat, "I will ban you right now! Just kidding");

        //Answer on message that satisfy provided regex expression
        [RegexTextMessage("^.*?(?i)python$")]
        public async Task Task() => await Bot.SendTextMessageAsync(Chat, "I hate snakes");

        //Answer on any update
        [Update(UpdateFlag.All)]
        public async Task Update() => await Bot.SendTextMessageAsync(Chat, "Hello");

        [Update(UpdateFlag.CallbackQuery)]
        public async Task CBQuery()
        {
            await Bot.SendTextMessageAsync(Chat, "callback");
        }

        [Command("query", CommandParseMode.Both)]
        public async Task HandleQuery()
        {
            var buttons = new List<InlineKeyboardButton>()
                {
                    new("Test")
                        {
                            CallbackData = "test_callback"
                        }
                };

            await Bot.SendTextMessageAsync(Chat.Id, "123", replyMarkup: new InlineKeyboardMarkup(buttons));
        }

        //Answer on message that contains photo or video
        [Message(MessageFlag.HasPhoto | MessageFlag.HasVideo)]
        public async Task PhotoVideo() => await Bot.SendTextMessageAsync(Chat, "Send me more!");

        //Answer on command with parameters: "/me hello"
        [ParametrizedCommand("me", CommandParseMode.Both)]
        public async Task Me(MeParam me)
        {
            var user = From.FirstName;
            if(!string.IsNullOrEmpty(From.LastName))
            {
                user += $" {From.LastName}";
            }
            await Bot.SendTextMessageAsync(Chat, $"<code>{user} says: {me.Text}</code>", parseMode: ParseMode.Html);
        }

        //Parametrized command with int parameter: "/status 2"
        [ParametrizedCommand("status", CommandParseMode.Both)]
        public async Task Status(int status)
        {
            await Bot.SendTextMessageAsync(Chat, "status " + status);
        }

        [Command("command1", CommandParseMode.WithUsername)]
        public async Task Command1()
        {
            await Bot.SendTextMessageAsync(Chat, "Command1");
        }

        [Message(MessageFlag.IsReply)]
        [Command("command2")]
        public async Task Command2()
        {
            await Bot.SendTextMessageAsync(Chat, "Command2");
        }

        [HandleCondition(ConditionType.Any)]
        [Message(MessageFlag.HasPhoto)]
        [Message(MessageFlag.HasSticker)]
        [Update(UpdateFlag.Message)]
        [Message(MessageFlag.HasText)]
        public async Task<bool> MultiAttr()
        {
            await Bot.SendTextMessageAsync(Chat.Id, "multi attributes");
            return true;
        }

        [HandleCondition(ConditionType.All)]
        [Message(MessageFlag.HasPhoto)]
        [Message(MessageFlag.HasCaption)]
        public async Task<bool> MultiAttr2()
        {
            await Bot.SendTextMessageAsync(Chat.Id, "Message with photo AND caption");
            return true;
        }

    }

    public class MeParam
    {
        public string Text;
    }

    //Custom parser for command parameters (for ParametrizedCommandAttribute)
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
