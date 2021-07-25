using System.Text.RegularExpressions;
using BotFramework.Setup;
using BotFramework.Enums;
using System;
using Telegram.Bot.Types;

namespace BotFramework.Attributes
{
    public class MessageAttribute:UpdateAttribute
    {
        internal MessageFlag MessageFlags;
        internal string Text;
        private bool isRegex = false;

        public MessageAttribute(InChat inChat, string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
        {
            isRegex = regex;
            InChat = inChat;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;
        }


        public MessageAttribute(string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
        {
            isRegex = regex;
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.Message;
            Text = text;
            MessageFlags = messageFlags;
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

            if(CanHandleMessage(message))
            {
                if(MessageFlags.HasFlag(MessageFlag.HasText))
                    return param.IsCommand || IsTextMatch(message.Text);

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
                    MessageFlag.HasForward      => message.ForwardFrom != null || message.ForwardFromChat != null,
                    MessageFlag.IsReply         => message.ReplyToMessage != null,
                    MessageFlag.HasText         => !string.IsNullOrEmpty(message.Text),
                    MessageFlag.HasEntity       => message.Entities != null || message.CaptionEntities != null,
                    MessageFlag.HasVoice        => message.Voice != null,
                    MessageFlag.HasAudio        => message.Audio != null,
                    MessageFlag.HasVideo        => message.Video != null,
                    MessageFlag.HasDocument     => message.Document != null,
                    MessageFlag.HasAnimation    => message.Animation != null,
                    MessageFlag.HasGame         => message.Game != null,
                    MessageFlag.HasCaption      => !string.IsNullOrEmpty(message.Caption),
                    MessageFlag.HasPhoto        => message.Photo != null,
                    MessageFlag.HasSticker      => message.Sticker != null,
                    MessageFlag.HasVideoNote    => message.VideoNote != null,
                    MessageFlag.HasContact      => message.Contact != null,
                    MessageFlag.HasLocation     => message.Location != null,
                    MessageFlag.HasVenue        => message.Venue != null,
                    MessageFlag.HasPoll         => message.Poll != null,
                    MessageFlag.HasDice         => message.Dice != null,
                    MessageFlag.HasKeyboard     => message.ReplyMarkup != null,
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
