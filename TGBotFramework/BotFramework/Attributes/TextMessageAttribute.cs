using System;
using System.Text.RegularExpressions;
using BotFramework.Enums;


namespace BotFramework.Attributes
{
    public class TextMessageAttribute: MessageAttribute
    {
        internal string Text { get; set; }
        internal TextContent TextContent { get; set; }

        internal StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        public string MessageText { get; set; }

        internal virtual bool IsCommand { get; set; } = false;

        internal virtual bool IsRegex { get; set; } = false;

        internal override bool IsTextMessage => true;

        public TextMessageAttribute() { }

        public TextMessageAttribute(TextContent textContent)
        {
            TextContent = textContent;
        }

        public TextMessageAttribute(string text)
        {
            Text = text;
        }
        public TextMessageAttribute(string text, TextContent textContent) : this(text)
        {
            TextContent = textContent;
        }
        public TextMessageAttribute(string text, StringComparison comparison) : this(text)
        {
            StringComparison = comparison;
        }


        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }
            var message = param.Update.Message;
            if(message == null)
            {
                return false;
            }
            switch(TextContent)
            {
                case TextContent.Text:
                    {
                        MessageText = message.Text;
                        break;
                    }
                case TextContent.Caption:
                    {
                        MessageText = message.Caption;
                        break;
                    }
                case TextContent.TextAndCaption:
                    {
                        MessageText = message.Text ?? message.Caption;
                        break;
                    }
            }
            if(string.IsNullOrEmpty(MessageText))
            {
                return false;
            }
            if(string.IsNullOrEmpty(Text))
            {
                return true;
            }

            if(IsRegex || (param.HasCommands || param.IsParametrizedCommand) && IsCommand)
                return true;

            return Text.Equals(MessageText, StringComparison);

        }
    }
}