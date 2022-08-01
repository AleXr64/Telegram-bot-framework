using BotFramework.Enums;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes
{
    public class ParametrizedCommandAttribute: CommandAttribute
    {

        public ParametrizedCommandAttribute()
            : base()
        {
            IsParametrized = true;
        }

        public ParametrizedCommandAttribute(string text)
            : base(text)
        {
            IsParametrized = true;
        }

        public ParametrizedCommandAttribute(InChat type, string text)
            : base(type, text)
        {
            IsParametrized = true;
        }

        public ParametrizedCommandAttribute(string text, CommandParseMode parseMode)
            : base(text, parseMode)
        {
            IsParametrized = true;
        }

        public ParametrizedCommandAttribute(InChat type, string text, CommandParseMode parseMode)
            : base(type, text, parseMode)
        {
            IsParametrized = true;
        }

        protected override bool CanHandle(HandlerParams param) => base.CanHandle(param) && IsEqual(param);

        private bool IsEqual(HandlerParams hParams)
        {
            if(!hParams.IsParametrizedCommand)
                return false;

            if(Mode == CommandParseMode.WithUsername && (!hParams.ParametrizedCmd.IsFullCommand || hParams.Chat.Type == ChatType.Private)
               || Mode == CommandParseMode.WithoutUsername && hParams.ParametrizedCmd.IsFullCommand)
            return false;


            return Text.Equals(hParams.ParametrizedCmd.Name);
        }
    }
}
