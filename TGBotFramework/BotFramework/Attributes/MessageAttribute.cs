using BotFramework.Enums;
using BotFramework.Setup;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MessageAttribute: UpdateAttribute
    {
        internal MessageFlag MessageFlags;

        internal virtual bool IsTextMessage { get; set; } = false;

        public MessageAttribute() : base(UpdateFlag.Message)
        {
            MessageFlags = MessageFlag.All;
        }


        public MessageAttribute(MessageFlag messageFlags)
            : this()
        {
            MessageFlags = messageFlags;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }

            var message = param.Update.Message;

            if(message != null)
            {
                if(IsTextMessage)
                {
                    return true;
                }
                return CanHandleMessage(message);
            }
            return false;
        }

        private bool CanHandleMessage(Message message)
        {
            return MessageFlags switch
                {
                    MessageFlag.All => true,
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
                };
        }



    }
}
