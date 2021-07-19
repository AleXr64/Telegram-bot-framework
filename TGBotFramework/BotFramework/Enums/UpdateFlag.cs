using System;

namespace BotFramework.Enums
{
    [Flags]
    public enum UpdateFlag
    {
        Message = 1 << 0,
        InlineQuery = 1 << 1,
        ChosenInlineResult = 1 << 2,
        CallbackQuery = 1 << 3,
        EditedMessage = 1 << 4,
        ChannelPost = 1 << 5,
        EditedChannelPost = 1 << 6,
        ShippingQuery = 1 << 7,
        PreCheckoutQuery = 1 << 8,
        Poll = 1 << 9,
        PollAnswer = 1 << 10,

        All = Message |
              InlineQuery |
              ChosenInlineResult |
              CallbackQuery |
              EditedMessage |
              ChannelPost |
              EditedChannelPost |
              ShippingQuery |
              PreCheckoutQuery |
              Poll |
              PollAnswer
    }
}
