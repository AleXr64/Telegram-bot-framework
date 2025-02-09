using BotFramework.Enums;
using System.Linq;
using Telegram.Bot.Types;

namespace BotFramework.Attributes
{
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
                return IsTextMessage || CanHandleMessage(message);
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
                    MessageFlag.HasEntity => message.Entities != null || message.CaptionEntities != null,
                    MessageFlag.HasCaption => !string.IsNullOrEmpty(message.Caption),
                    MessageFlag.HasKeyboard => message.ReplyMarkup != null,
                    MessageFlag.HasMediaSpoiler => message.HasMediaSpoiler,

                    //Generated from MessageType
                    MessageFlag.HasText => !string.IsNullOrEmpty(message.Text),
                    MessageFlag.HasAnimation => message.Animation != null,
                    MessageFlag.HasAudio => message.Audio != null,
                    MessageFlag.HasDocument => message.Document != null,
                    MessageFlag.HasPaidMedia => message.PaidMedia != null,
                    MessageFlag.HasPhoto => message.Photo != null,
                    MessageFlag.HasSticker => message.Sticker != null,
                    MessageFlag.HasStory => message.Story != null,
                    MessageFlag.HasVideo => message.Video != null,
                    MessageFlag.HasVideoNote => message.VideoNote != null,
                    MessageFlag.HasVoice => message.Voice != null,
                    MessageFlag.HasContact => message.Contact != null,
                    MessageFlag.HasDice => message.Dice != null,
                    MessageFlag.HasGame => message.Game != null,
                    MessageFlag.HasPoll => message.Poll != null,
                    MessageFlag.HasVenue => message.Venue != null,
                    MessageFlag.HasLocation => message.Location != null,
                    MessageFlag.HasNewChatMembers => message.NewChatMembers?.Any() == true,
                    MessageFlag.HasLeftChatMember => message.LeftChatMember != null,
                    MessageFlag.HasNewChatTitle => !string.IsNullOrEmpty(message.NewChatTitle),
                    MessageFlag.HasNewChatPhoto => message.NewChatPhoto != null,
                    MessageFlag.HasDeleteChatPhoto => message.DeleteChatPhoto != null,
                    MessageFlag.HasGroupChatCreated => message.GroupChatCreated != null,
                    MessageFlag.HasSupergroupChatCreated => message.SupergroupChatCreated != null,
                    MessageFlag.HasChannelChatCreated => message.ChannelChatCreated != null,
                    MessageFlag.HasAutoDeleteTimerChanged => message.MessageAutoDeleteTimerChanged != null,
                    MessageFlag.HasMigrateToChatId => message.MigrateToChatId != null,
                    MessageFlag.HasMigrateFromChatId => message.MigrateFromChatId != null,
                    MessageFlag.HasPinnedMessage => message.PinnedMessage != null,
                    MessageFlag.HasInvoice => message.Invoice != null,
                    MessageFlag.HasSuccessfulPayment => message.SuccessfulPayment != null,
                    MessageFlag.HasRefundedPayment => message.RefundedPayment != null,
                    MessageFlag.HasUsersShared => message.UsersShared != null,
                    MessageFlag.HasChatShared => message.ChatShared != null,
                    MessageFlag.HasConnectedWebsite => message.ConnectedWebsite != null,
                    MessageFlag.WriteAccessAllowed => message.WriteAccessAllowed != null,
                    MessageFlag.HasPassportData => message.PassportData != null,
                    MessageFlag.HasProximityAlert => message.ProximityAlertTriggered != null,
                    MessageFlag.HasBoostAdded => message.BoostAdded != null,
                    MessageFlag.HasChatBackgroundSet => message.ChatBackgroundSet != null,
                    MessageFlag.HasForumTopicCreated => message.ForumTopicCreated != null,
                    MessageFlag.HasForumTopicEdited => message.ForumTopicEdited != null,
                    MessageFlag.HasForumTopicClosed => message.ForumTopicClosed != null,
                    MessageFlag.HasForumTopicReopened => message.ForumTopicReopened != null,
                    MessageFlag.HasGeneralForumTopicHidden => message.GeneralForumTopicHidden != null,
                    MessageFlag.HasGeneralForumTopicUnhidden => message.GeneralForumTopicUnhidden != null,
                    MessageFlag.HasGiveawayCreated => message.GiveawayCreated != null,
                    MessageFlag.HasGiveaway => message.Giveaway != null,
                    MessageFlag.HasGiveawayWinners => message.GiveawayWinners != null,
                    MessageFlag.HasGiveawayCompleted => message.GiveawayCompleted != null,
                    MessageFlag.HasVideoChatScheduled => message.VideoChatScheduled != null,
                    MessageFlag.HasVideoChatStarted => message.VideoChatStarted != null,
                    MessageFlag.HasVideoChatEnded => message.VideoChatEnded != null,
                    MessageFlag.HasVideoChatParticipantsInvited => message.VideoChatParticipantsInvited != null,

                    _ => false
                };
        }



    }
}
