using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Attributes;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotFramework.Enums;
using Telegram.Bot;
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
        [Command(InChat.Private, "start", CommandParseMode.Both), Priority(10)]
        public async Task<bool> Start()
        {
            await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");
            return false;
        }

        //Answer on message with "ban" text
        [Message("ban")]
        public async Task Ban() => await Bot.SendTextMessageAsync(Chat, "I will ban you right now! Just kidding");

        //Answer on message that satisfy provided regex expression
        [Message("^.*?(?i)python$", regex: true, messageFlags:MessageFlag.HasCaption | MessageFlag.HasText)]
        public async Task Task() => await Bot.SendTextMessageAsync(Chat, "I hate snakes");

        //Answer on any update
        [Update(InChat.All, UpdateFlag.All)]
        public async Task Update() => await Bot.SendTextMessageAsync(Chat, "Hello");

        [Update(InChat.Public, UpdateFlag.CallbackQuery)]
        public async Task CBQuery()
        {
            await Bot.SendTextMessageAsync(Chat, "callback");
        }

        [Command(InChat.Public, "query", CommandParseMode.Both)]
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
        [Message(InChat.All, MessageFlag.HasPhoto | MessageFlag.HasVideo)]
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

        [Command("command2")]
        public async Task Command2()
        {
            await Bot.SendTextMessageAsync(Chat, "Command2");
        }

        [Command("spoiler")]
        public async Task Spoiler()
        {
            await Bot.SendAnimationAsync(Chat.Id, animation: new InputFileUrl(@"https://file-examples.com/storage/fecd197fb063b33dd9d79e6/2017/04/file_example_MP4_480_1_5MG.mp4"), hasSpoiler: true);
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
