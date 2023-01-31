using System;
using BotFramework.Enums;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class ParametrizedCommandAttribute : CommandAttribute
    {
        internal override bool IsParametrized => true;
        public ParametrizedCommandAttribute() { }

        public ParametrizedCommandAttribute(string text)
            : base(text)  { }

        public ParametrizedCommandAttribute(string text, TextContent textContent)
            : base(text, textContent)  { }

        public ParametrizedCommandAttribute(string text, CommandParseMode parseMode)
            : base(text, parseMode)  { }

        public ParametrizedCommandAttribute(string text, CommandParseMode parseMode, TextContent textContent)
            : base(text, parseMode, textContent) { }

        protected override bool CanHandle(HandlerParams param) => base.CanHandle(param) && IsEqual(param);

        private bool IsEqual(HandlerParams hParams)
        {
            if(!hParams.IsParametrizedCommand)
                return false;

            return (Mode, hParams.ParametrizedCmd.IsFullCommand) switch
                {
                    (CommandParseMode.WithUsername, false) => false,
                    (CommandParseMode.WithoutUsername, true) => false,
                    _ => Text.Equals(hParams.ParametrizedCmd.Name)
                };
        }
    }
}
