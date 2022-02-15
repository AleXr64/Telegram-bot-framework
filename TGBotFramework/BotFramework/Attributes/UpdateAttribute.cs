using BotFramework.Enums;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class UpdateAttribute:HandlerAttribute
    {
        internal UpdateFlag UpdateFlags;
        internal InChat InChatFlags;

        public UpdateAttribute()
        {
            InChatFlags = InChat.All;
            UpdateFlags = UpdateFlag.All;
        }
        public UpdateAttribute(InChat inChatFlags, UpdateFlag updateFlags)
        {
            InChatFlags = inChatFlags;
            UpdateFlags = updateFlags;
        }

        protected override bool CanHandle(HandlerParams param)
        {

            if(!CanHandleChat(param.InChat))
                return false;

            if(UpdateFlags.HasFlag(UpdateFlag.All))
                return true;

            return param.Type switch
                {
                    UpdateType.Message => UpdateFlags.HasFlag(UpdateFlag.Message),
                    UpdateType.InlineQuery => UpdateFlags.HasFlag(UpdateFlag.InlineQuery),
                    UpdateType.ChosenInlineResult => UpdateFlags.HasFlag(UpdateFlag.ChosenInlineResult),
                    UpdateType.CallbackQuery => UpdateFlags.HasFlag(UpdateFlag.CallbackQuery),
                    UpdateType.EditedMessage => UpdateFlags.HasFlag(UpdateFlag.EditedMessage),
                    UpdateType.ChannelPost => UpdateFlags.HasFlag(UpdateFlag.ChannelPost),
                    UpdateType.EditedChannelPost => UpdateFlags.HasFlag(UpdateFlag.EditedChannelPost),
                    UpdateType.ShippingQuery => UpdateFlags.HasFlag(UpdateFlag.ShippingQuery),
                    UpdateType.PreCheckoutQuery => UpdateFlags.HasFlag(UpdateFlag.PreCheckoutQuery),
                    UpdateType.Poll => UpdateFlags.HasFlag(UpdateFlag.Poll),
                    UpdateType.PollAnswer => UpdateFlags.HasFlag(UpdateFlag.PollAnswer),
                    UpdateType.MyChatMember => UpdateFlags.HasFlag(UpdateFlag.MyChatMember),
                    UpdateType.ChatMember => UpdateFlags.HasFlag(UpdateFlag.ChatMember),
                    _ => false
                };
        }

        private bool CanHandleChat(InChat flags)
        {
            if(InChatFlags == InChat.All)
                return true;

            return InChatFlags switch
                {
                    InChat.Public => flags.HasFlag(InChat.Public),
                    InChat.Private => flags.HasFlag(InChat.Private),
                    InChat.Channel => flags.HasFlag(InChat.Channel),
                    _ => false
                };
        }

    }
}
