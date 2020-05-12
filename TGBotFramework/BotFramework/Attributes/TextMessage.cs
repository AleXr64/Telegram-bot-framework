using BotFramework.Setup;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class TextMessage: HandlerAttribute
    {
        private readonly InChat _inChat;
        private readonly string _text;
        private readonly bool Empty;
        private readonly bool _regexp = false;

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

        public TextMessage(InChat inChat, string text, bool regexp)
        {
            _inChat = inChat;
            _text = text;
            _regexp = regexp;
        }

        public TextMessage(InChat inChat, bool regexp)
        {
            _text = "";
            _inChat = inChat;
            _regexp = regexp;
        }

        public TextMessage(string text, bool regexp)
        {
            _text = text;
            _inChat = InChat.All;
            _regexp = regexp;
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

            if(_inChat != InChat.All) 
                chatMatch = param.InChat == _inChat;
            else
                chatMatch = true;

            if(Empty || string.IsNullOrEmpty(_text))
                return chatMatch;

            if(_regexp)
                textMatch = Regex.IsMatch(messageText, _text);
            else
                textMatch = _text == messageText;

            return textMatch && chatMatch;
        }
    }
}
