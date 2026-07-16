using System;
using System.Collections.Generic;
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
            
            if(Text == null)
                return true;
            
            var commands = new List<HandlerParams.Command>() { hParams.ParametrizedCmd };
            return CanHandleChatType(hParams.Chat.Type, commands);
        }
    }
}
