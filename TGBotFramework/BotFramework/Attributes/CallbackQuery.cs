using BotFramework.Setup;

namespace BotFramework.Attributes
{
    public class CallbackQueryAttribute: HandlerAttribute
    {
        private readonly InChat _inChat;
        private readonly bool requreChat;

        public CallbackQueryAttribute(InChat inChat)
        {
            _inChat = inChat;
            requreChat = true;
        }

        public CallbackQueryAttribute() { }

        protected override bool CanHandle(HandlerParams param)
        {
            if(requreChat) return param.CallbackQuery != null && param.InChat == _inChat;
            return param.CallbackQuery != null;
        }
    }
}
