using System;
using System.Linq;
using BotFramework.Enums;
using BotFramework.Setup;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class CommandAttribute:MessageAttribute
    {
        internal readonly CommandParseMode Mode;
        internal bool IsParametrized = false;
        public CommandAttribute()
        {
            MessageFlags = MessageFlag.HasText | MessageFlag.HasCaption;
            IsCommand = true;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="inChatFlags">Which chat type for handle <see cref="InChat" /></param>
        /// <param name="text">Command name</param>
        public CommandAttribute(InChat inChatFlags, string text) : this()
        {
            InChatFlags = inChatFlags;
            Text = text;
            Mode = CommandParseMode.Both;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        public CommandAttribute(string text) : this()
        {
            Text = text;
            InChatFlags = InChat.All;
            Mode = CommandParseMode.Both;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        /// <param name="parseMode">Command parse mode <see cref="CommandParseMode" /></param>
        public CommandAttribute(string text, CommandParseMode parseMode) : this()
        {
            Text = text;
            InChatFlags = InChat.All;
            Mode = parseMode;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="inChatFlags">>Which chat type for handle <see cref="InChat" /></param>
        /// <param name="text">Command name</param>
        /// <param name="parseMode">Command parse mode <see cref="CommandParseMode" /></param>
        public CommandAttribute(InChat inChatFlags, string text, CommandParseMode parseMode) : this()
        {
            InChatFlags = inChatFlags;
            Text = text;
            Mode = parseMode;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            return base.CanHandle(param) && IsCommandEqual(param);
        }

        private bool IsCommandEqual(HandlerParams hParams)
        {
            if(IsParametrized)
                return true;

            if(hParams.Chat.Type == ChatType.Private ||
               Mode == CommandParseMode.Both)

            {

                return hParams.Commands.Any(x => x.Name.Equals(Text));
            }

            return hParams.Commands.Any(x => x.Name.Equals(Text) &&
                                             (Mode == CommandParseMode.WithUsername
                                                 ? x.IsFullCommand
                                                 : !x.IsFullCommand));
        }
    }
}
