using System;
using BotFramework.Enums;
using BotFramework.Setup;
namespace BotFramework.Attributes
{
    public class CommandAttribute:MessageAttribute
    {
        private readonly CommandParseMode _mode;

        public CommandAttribute()
        {
            MessageFlags = MessageFlag.HasText;
            IsCommand = true;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="inChat">Which chat type for handle <see cref="InChat" /></param>
        /// <param name="text">Command name</param>
        public CommandAttribute(InChat inChat, string text) : this()
        {
            InChat = inChat;
            Text = text;
            _mode = CommandParseMode.WithUsername;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        public CommandAttribute(string text) : this()
        {
            Text = text;
            InChat = InChat.All;
            _mode = CommandParseMode.WithUsername;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        /// <param name="parseMode">Command parse mode <see cref="CommandParseMode" /></param>
        public CommandAttribute(string text, CommandParseMode parseMode) : this()
        {
            Text = text;
            InChat = InChat.All;
            _mode = parseMode;
        }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="inChat">>Which chat type for handle <see cref="InChat" /></param>
        /// <param name="text">Command name</param>
        /// <param name="parseMode">Command parse mode <see cref="CommandParseMode" /></param>
        public CommandAttribute(InChat inChat, string text, CommandParseMode parseMode) : this()
        {
            InChat = inChat;
            Text = text;
            _mode = parseMode;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            return base.CanHandle(param) && IsCommandEqual(param);
        }

        private bool IsCommandEqual(HandlerParams hParams)
        {
            switch(_mode)
            {
                case CommandParseMode.WithUsername:
                    return string.Equals(Text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase) &&
                           hParams.IsFullFormCommand;

                case CommandParseMode.WithoutUsername:
                    return string.Equals(Text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase) &&
                           !hParams.IsFullFormCommand;

                case CommandParseMode.Both:
                    return string.Equals(Text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
