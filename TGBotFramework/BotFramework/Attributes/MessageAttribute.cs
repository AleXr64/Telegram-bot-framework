using BotFramework.Enums;
using BotFramework.Setup;
using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace BotFramework.Attributes
{
    public class MessageAttribute: UpdateAttribute
    {
        internal MessageFlag MessageFlags;
        internal string Text;
        internal bool IsCommand = false;
        private bool isRegex = false;

        public MessageAttribute()
        {
            UpdateFlags = UpdateFlag.Message;
            InChat = InChat.All;
            MessageFlags = MessageFlag.All;
        }
        public MessageAttribute(InChat inChat, string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
            : this()
        {
            isRegex = regex;
            InChat = inChat;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
            : this()
        {
            isRegex = regex;
            InChat = InChat.All;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(InChat inChat, MessageFlag messageFlags)
            : this()
        {
            InChat = inChat;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(MessageFlag messageFlags)
            : this()
        {
            InChat = InChat.All;
            MessageFlags = messageFlags;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }

            var message = param.Update.Message;

            if(CanHandleMessage(message))
            {
                if(MessageFlags.HasFlag(MessageFlag.HasText))
                {
                    if(param.IsCommand && IsCommand)
                        return true;
                    return IsTextMatch(message.Text);
                }
                return true;
            }
            return false;
        }

        private bool CanHandleMessage(Message message)
        {
            if(MessageFlags.HasFlag(MessageFlag.All))
            {
                return true;
            }

            foreach(var f in Enum.GetValues<MessageFlag>())
            {
                if(!MessageFlags.HasFlag(f))
                    continue;

                var ret = f switch
                    {
                        MessageFlag.HasForward => message.ForwardFrom != null || message.ForwardFromChat != null,
                        MessageFlag.IsReply => message.ReplyToMessage != null,
                        MessageFlag.HasText => !string.IsNullOrEmpty(message.Text),
                        MessageFlag.HasEntity => message.Entities != null || message.CaptionEntities != null,
                        MessageFlag.HasVoice => message.Voice != null,
                        MessageFlag.HasAudio => message.Audio != null,
                        MessageFlag.HasVideo => message.Video != null,
                        MessageFlag.HasDocument => message.Document != null,
                        MessageFlag.HasAnimation => message.Animation != null,
                        MessageFlag.HasGame => message.Game != null,
                        MessageFlag.HasCaption => !string.IsNullOrEmpty(message.Caption),
                        MessageFlag.HasPhoto => message.Photo != null,
                        MessageFlag.HasSticker => message.Sticker != null,
                        MessageFlag.HasVideoNote => message.VideoNote != null,
                        MessageFlag.HasContact => message.Contact != null,
                        MessageFlag.HasLocation => message.Location != null,
                        MessageFlag.HasVenue => message.Venue != null,
                        MessageFlag.HasPoll => message.Poll != null,
                        MessageFlag.HasDice => message.Dice != null,
                        MessageFlag.HasKeyboard => message.ReplyMarkup != null,
                        MessageFlag.HasNewChatMembers => message.NewChatMembers != null && message.NewChatMembers?.Length != 0,
                        MessageFlag.HasLeftChatMember => message.LeftChatMember != null,
                        _ => false
                    };
                if(ret)
                    return true;
            }
            return false;
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
