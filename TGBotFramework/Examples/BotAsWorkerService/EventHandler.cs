using System;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Attributes;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotFramework.Enums;

namespace BotAsWorkerService
{
    public class EventHandler:BotEventHandler
    {
        // Answer on "/start" command from private messages
        [Command(InChat.Private, "start", CommandParseMode.Both), Priority(1)]
        public async Task Start()
        {
            await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");
            return ;
        }

        //Answer on message with "ban" text
        [Message("ban")]
        public async Task Ban() => await Bot.SendTextMessageAsync(Chat, "I will ban you right now! Just kidding");

        //Answer on message that satisfy provided regex expression
        [Message("^.*?(?i)python$", regex: true)]
        public async Task Task() => await Bot.SendTextMessageAsync(Chat, "I hate snakes");

        //Answer on any update
        [Update(InChat.All, UpdateFlag.All)]
        public async Task Update() => await Bot.SendTextMessageAsync(Chat, "Hello");

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
            await Bot.SendTextMessageAsync(Chat, $"<code>{user} says: {me.Text}</code>", ParseMode.Html);
        }
    }

    public class MeParam
    {
        public string Text;
    }

    //Custom parser for command parameters (for ParametrizedCommand)
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
