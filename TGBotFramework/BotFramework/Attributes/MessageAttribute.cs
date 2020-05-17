using System;
using System.Text.RegularExpressions;
using BotFramework.Setup;

namespace BotFramework.Attributes
{
    public class MessageAttribute:UpdateAttribute
    {
        [Flags]
        public enum MessageFlag
        {
            HasForward,
            IsReply,
            HasText,
            HasEntity,
            HasAudio,
            HasDocument,
            HasAnimation,
            HasGame,
            HasPhoto,
            HasSticker,
            HasVideo,
            HasVoice,
            HasVideoNote,
            HasCaption,
            HasContact,
            HasLocation,
            HasVenue,
            HasPoll,
            HasDice,
            HasKeyboard,
            All = HasForward | IsReply | HasText| HasEntity | HasAudio | HasDocument | HasAnimation | HasGame | HasPhoto | HasSticker | HasVideo | HasVoice | HasVideoNote | HasCaption | HasContact | HasLocation | HasVenue | HasPoll | HasDice | HasKeyboard
        }

        internal MessageFlag MessageFlags;
        internal string Text;
        private bool isRegex = false;

        public MessageAttribute(InChat inChat, string text, MessageFlag messageFlags, bool regex)
        {
            isRegex = regex;
            InChat = inChat;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(InChat inChat, string text, MessageFlag messageFlags)
        {
            InChat = inChat;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;
        }


        public MessageAttribute(string text, MessageFlag messageFlags, bool regex)
        {
            isRegex = regex;
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(string text, MessageFlag messageFlags)
        {
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;//TODO:add HasText to flags?
        }


        public MessageAttribute(string text, bool regex)
        {
            isRegex = regex;
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = MessageFlag.HasText;
        }

        public MessageAttribute(string text)
        {
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = MessageFlag.HasText;
        }

        public MessageAttribute(InChat inChat, MessageFlag messageFlags)
        {
            InChat = inChat;
            UpdateFlags = UpdateFlag.Message;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(MessageFlag messageFlags)
        {
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            MessageFlags = messageFlags;
        }

        public MessageAttribute()
        {
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            MessageFlags = MessageFlag.All;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }

            var message = param.Update.Message;

            if(IsMessageMatch(message))
            {
                if(MessageFlags.HasFlag(MessageFlag.HasText))
                    return param.IsCommand || IsTextMatch(message.Text);

                return true;
            }
            return false;
        }

        private bool IsMessageMatch(Telegram.Bot.Types.Message message)
        {
            if(MessageFlags.HasFlag(MessageFlag.All))
            {
                return true;
            }

            var messageMatch = false;
            if(MessageFlags.HasFlag(MessageFlag.HasForward))
            {
                messageMatch = (message.ForwardFrom != null || message.ForwardFromChat != null);
            }

            if(MessageFlags.HasFlag(MessageFlag.IsReply))
            {
                messageMatch = message.ReplyToMessage != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasText))
            {
                messageMatch = !string.IsNullOrEmpty(message.Text);
            }

            if(MessageFlags.HasFlag(MessageFlag.HasEntity))
            {
                messageMatch = message.Entities != null || message.CaptionEntities != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasAudio))
            {
                messageMatch = message.Audio != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasDocument))
            {
                messageMatch = message.Document != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasAnimation))
            {
                messageMatch = message.Animation != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasGame))
            {
                messageMatch = message.Game != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasPhoto))
            {
                messageMatch = message.Photo != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasSticker))
            {
                messageMatch = message.Sticker != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasVideo))
            {
                messageMatch = message.Video != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasVoice))
            {
                messageMatch = message.Voice != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasVideoNote))
            {
                messageMatch = message.VideoNote != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasCaption))
            {
                messageMatch = !string.IsNullOrEmpty(message.Caption);
            }

            if(MessageFlags.HasFlag(MessageFlag.HasContact))
            {
                messageMatch = message.Contact != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasLocation))
            {
                messageMatch = message.Location != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasVenue))
            {
                messageMatch = message.Venue != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasPoll))
            {
                messageMatch = message.Poll != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasDice))
            {
                messageMatch = message.Dice != null;
            }

            if(MessageFlags.HasFlag(MessageFlag.HasKeyboard))
            {
                messageMatch = message.ReplyMarkup != null;
            }

            return messageMatch;
        }

        private bool IsTextMatch(string text)
        {
            if(string.IsNullOrEmpty(Text))
            {
                return true;
            }

            if(string.IsNullOrEmpty(text))
            {
                return false;
            }

            return isRegex ? Regex.IsMatch(text, Text) : Text.Equals(text);
        }
    }
}
