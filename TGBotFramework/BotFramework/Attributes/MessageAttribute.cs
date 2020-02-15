using BotFramework.Setup;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class MessageAttribute: HandlerAttribute
    {
        public InChat Chat;
        public MessageType MessageType;

        public MessageAttribute(MessageType messageType, InChat chat)
        {
            MessageType = messageType;
            Chat = chat;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(param.Type == UpdateType.Message)
                if(param.InChat == Chat && param.Update.Message.Type == MessageType)
                    return true;
            return false;
        }
    }
}
