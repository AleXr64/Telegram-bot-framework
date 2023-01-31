using System;
using System.Text.RegularExpressions;
using BotFramework.Enums;


namespace BotFramework.Attributes
{
    public class TextMessageAttribute: MessageAttribute
    {
        internal override bool IsTextMessage => true;

        /// <summary>
        /// Text pattern to compare with message
        /// </summary>
        internal string Text { get; set; }

        /// <summary>
        /// What content should be compared: Caption, Text or both.
        /// For details see: <see cref="TextContent"/>
        /// </summary>
        internal TextContent TextContent { get; set; }
        /// <summary>
        /// String comparison type for `string.Equals()`
        /// </summary>
        internal StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
        
        /// <summary>
        /// Text or caption of message
        /// </summary>
        public string MessageText { get; set; }


        internal virtual bool IsCommand { get; set; } = false;

        internal virtual bool IsRegex { get; set; } = false;



        public TextMessageAttribute() { }

        public TextMessageAttribute(TextContent textContent)
        {
            TextContent = textContent;
        }

        public TextMessageAttribute(string text)
        {
            Text = text;
        }
        public TextMessageAttribute(string text, TextContent textContent)
            : this(text)
        {
            TextContent = textContent;
        }
        public TextMessageAttribute(string text, StringComparison comparison)
            : this(text)
        {
            StringComparison = comparison;
        }

        public TextMessageAttribute(string text, TextContent textContent, StringComparison comparison)
            : this(text, textContent)
        {
            StringComparison = comparison;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            var message = param.Update.Message;
            if(!base.CanHandle(param) || message == null)
            {
                return false;
            }

            MessageText = TextContent switch
                {
                    TextContent.Text => message.Text,
                    TextContent.Caption => message.Caption,
                    TextContent.TextAndCaption => message.Text ?? message.Caption,
                    _ => MessageText
                };

            if(string.IsNullOrEmpty(MessageText))
            {
                return false;
            }

            if(string.IsNullOrEmpty(Text))
            {
                return true;
            }

            if(IsRegex || IsCommand)
                return true;

            return Text.Equals(MessageText, StringComparison);

        }
    }
}