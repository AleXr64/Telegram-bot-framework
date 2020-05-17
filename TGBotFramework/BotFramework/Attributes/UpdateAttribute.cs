using System;
using BotFramework.Setup;
using BotFramework.Utils;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class UpdateAttribute:HandlerAttribute
    {
        [Flags]
        public enum UpdateFlag
        {
            Message,
            InlineQuery,
            ChosenInlineResult,
            CallbackQuery,
            EditedMessage,
            ChannelPost,
            EditedChannelPost,
            ShippingQuery,
            PreCheckoutQuery,
            Poll,
            PollAnswer,
            All
        }

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

            var typeMatch = param.Type switch
                {
                    UpdateType.Message => UpdateFlags.IsSet(UpdateFlag.Message),
                    UpdateType.InlineQuery => UpdateFlags.IsSet(UpdateFlag.InlineQuery),
                    UpdateType.ChosenInlineResult => UpdateFlags.IsSet(UpdateFlag.ChosenInlineResult),
                    UpdateType.CallbackQuery => UpdateFlags.IsSet(UpdateFlag.CallbackQuery),
                    UpdateType.EditedMessage => UpdateFlags.IsSet(UpdateFlag.EditedMessage),
                    UpdateType.ChannelPost => UpdateFlags.IsSet(UpdateFlag.ChannelPost),
                    UpdateType.EditedChannelPost => UpdateFlags.IsSet(UpdateFlag.EditedChannelPost),
                    UpdateType.ShippingQuery => UpdateFlags.IsSet(UpdateFlag.ShippingQuery),
                    UpdateType.PreCheckoutQuery => UpdateFlags.IsSet(UpdateFlag.PreCheckoutQuery),
                    UpdateType.Poll => UpdateFlags.IsSet(UpdateFlag.Poll),
                    UpdateType.PollAnswer => UpdateFlags.IsSet(UpdateFlag.PollAnswer),
                    _ => false
                };

            return typeMatch;
        }
    }
}
