using System.Linq;
using BotFramework.Enums;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class CommandAttribute: TextMessageAttribute
    {
        internal readonly CommandParseMode Mode = CommandParseMode.Both;
        internal virtual bool IsParametrized { get; set; } = false;

        internal override bool IsCommand => true;
        public CommandAttribute() { }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        public CommandAttribute(string text) 
            : base(text) { }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textContent"></param>
        public CommandAttribute(string text, TextContent textContent)
            : base(text, textContent) { }

        /// <summary>
        ///     Marks method as handler for command without parameters
        /// </summary>
        /// <param name="text">Command name</param>
        /// <param name="parseMode">Command parse mode <see cref="CommandParseMode" /></param>
        public CommandAttribute(string text, CommandParseMode parseMode)
            : this(text)
        {
            Mode = parseMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="textContent"></param>
        public CommandAttribute(string text, CommandParseMode parseMode, TextContent textContent)
            : this(text, textContent)
        {
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

            if(!hParams.HasCommands)
                return false;
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
