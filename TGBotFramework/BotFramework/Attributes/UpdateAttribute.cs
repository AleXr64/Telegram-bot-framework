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
            All = Message | InlineQuery | ChosenInlineResult | CallbackQuery | EditedMessage | ChannelPost | EditedChannelPost | ShippingQuery | PreCheckoutQuery | Poll | PollAnswer
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

            var typeMatch = false;
            switch(param.Type)
                {
                    case UpdateType.Message:
                    {
                        typeMatch = UpdateFlags.HasFlag(UpdateFlag.Message) ;
                        break;
                    }
                    case UpdateType.InlineQuery:
                    {
                        typeMatch = UpdateFlags.HasFlag(UpdateFlag.InlineQuery);
                        break;
                    }
                    case UpdateType.ChosenInlineResult:
                    {
                        typeMatch = UpdateFlags.HasFlag(UpdateFlag.ChosenInlineResult);
                        break;
                    }
                    /*UpdateType.CallbackQuery => UpdateFlags.HasFlag(UpdateFlag.CallbackQuery),
                    UpdateType.EditedMessage => UpdateFlags.HasFlag(UpdateFlag.EditedMessage),
                    UpdateType.ChannelPost => UpdateFlags.HasFlag(UpdateFlag.ChannelPost),
                    UpdateType.EditedChannelPost => UpdateFlags.HasFlag(UpdateFlag.EditedChannelPost),
                    UpdateType.ShippingQuery => UpdateFlags.HasFlag(UpdateFlag.ShippingQuery),
                    UpdateType.PreCheckoutQuery => UpdateFlags.HasFlag(UpdateFlag.PreCheckoutQuery),
                    UpdateType.Poll => UpdateFlags.HasFlag(UpdateFlag.Poll),
                    UpdateType.PollAnswer => UpdateFlags.HasFlag(UpdateFlag.PollAnswer),
                    _ => false*/
                };

            return typeMatch;
        }
    }
}
