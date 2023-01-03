using BotFramework.Enums;
using BotFramework.Setup;
using System;
using System.Linq;
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
            InChatFlags = InChat.All;
            MessageFlags = MessageFlag.All;
        }
        public MessageAttribute(InChat inChatFlags, string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
            : this()
        {
            isRegex = regex;
            InChatFlags = inChatFlags;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(string text, MessageFlag messageFlags = MessageFlag.HasText, bool regex = false)
            : this()
        {
            isRegex = regex;
            InChatFlags = InChat.All;
            Text = text;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(InChat inChatFlags, MessageFlag messageFlags)
            : this()
        {
            InChatFlags = inChatFlags;
            MessageFlags = messageFlags;
        }

        public MessageAttribute(MessageFlag messageFlags)
            : this()
        {
            InChatFlags = InChat.All;
            MessageFlags = messageFlags;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }

            var message = param.Update.Message;

            if(message != null && CanHandleMessage(message))
            {
                var text = message.Text ?? message.Caption;
                if(string.IsNullOrEmpty(text) && string.IsNullOrEmpty(Text))
                    return true;

                if(!string.IsNullOrEmpty(text))
                {
                    if((param.HasCommands || param.IsParametrizedCommand) && IsCommand)//pass to command handler
                        return true;
                    return IsTextMatch(text);
                }
            }
            return false;
        }

        private bool CanHandleMessage(Message message)
        {
            if(MessageFlags.HasFlag(MessageFlag.All))
            {
                return true;
            }

            return (from object f in Enum.GetValues(typeof(MessageFlag))
                    where MessageFlags.HasFlag((Enum)f)
                    select f switch
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
                            MessageFlag.HasNewChatMembers => message.NewChatMembers?.Any() == true,
                            MessageFlag.HasLeftChatMember => message.LeftChatMember != null,
                            MessageFlag.HasNewChatTitle => !string.IsNullOrEmpty(message.NewChatTitle),
                            MessageFlag.HasNewChatPhoto => message.NewChatPhoto != null,
                            MessageFlag.HasDeleteChatPhoto => message.DeleteChatPhoto != null,
                            MessageFlag.HasGroupChatCreated => message.GroupChatCreated != null,
                            MessageFlag.HasSupergroupChatCreated => message.SupergroupChatCreated != null,
                            MessageFlag.HasPinnedMessage => message.PinnedMessage != null,
                            MessageFlag.HasVideoChatScheduled => message.VideoChatScheduled != null,
                            MessageFlag.HasVideoChatStarted => message.VideoChatStarted != null,
                            MessageFlag.HasVideoChatEnded => message.VideoChatEnded != null,
                            MessageFlag.HasVideoChatParticipantsInvited => message.VideoChatParticipantsInvited != null,
                            MessageFlag.HasMediaSpoiler => message.HasMediaSpoiler ?? false,
                            MessageFlag.HasInvoice => message.Invoice != null,
                            MessageFlag.HasPassportData => message.PassportData != null,
                            MessageFlag.HasSuccessfulPayment => message.SuccessfulPayment != null,
                            _ => false
                        }).Any(ret => ret);
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
