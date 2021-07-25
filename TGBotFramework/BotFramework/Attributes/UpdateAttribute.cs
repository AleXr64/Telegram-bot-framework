using System;
using BotFramework.Enums;
using BotFramework.Setup;
using BotFramework.Utils;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class UpdateAttribute:HandlerAttribute
    {
        internal UpdateFlag UpdateFlags;
        internal InChat InChat;

        public UpdateAttribute()
        {
            InChat = InChat.All;
            UpdateFlags = UpdateFlag.All;
        }
        public UpdateAttribute(InChat inChat, UpdateFlag updateFlags)
        {
            InChat = inChat;
            UpdateFlags = updateFlags;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(InChat != InChat.All && param.InChat != InChat)
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
                    _ => false
                };
        }
    }
}
