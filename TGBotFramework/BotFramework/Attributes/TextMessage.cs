using BotFramework.Setup;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class TextMessage: HandlerAttribute
    {
        private readonly InChat _inChat;
        private readonly string _text;
        private readonly bool Empty;

        public TextMessage() { Empty = true; }

        public TextMessage(InChat inChat, string text)
        {
            _inChat = inChat;
            _text = text;
        }

        public TextMessage(InChat inChat)
        {
            _text = "";
            _inChat = inChat;
        }

        public TextMessage(string text)
        {
            _text = text;
            _inChat = InChat.All;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(param.Type != UpdateType.Message || param.Update.Message.Type != MessageType.Text)
                return false;

            if(param.IsCommand)
                return false;

            var message = param.Update.Message;
            var messageText = message.Text;

            var chatMatch = false;
            var textMatch = false;

            if(_inChat != InChat.All) chatMatch = param.InChat == _inChat;

            if(!string.IsNullOrEmpty(_text)) textMatch = _text == messageText;

            return chatMatch || textMatch || Empty;
        }
    }
}
