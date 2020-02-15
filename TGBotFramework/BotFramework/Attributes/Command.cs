using System;
using BotFramework.Setup;

namespace BotFramework.Attributes
{
    public class Command: HandlerAttribute
    {
        private readonly InChat _type;
        private readonly string text;
        private readonly CommandParseMode mode;

        public Command(InChat type, string Text)
        {
            _type = type;
            text = Text;
            mode = CommandParseMode.WithUsername;
        }

        public Command(string Text)
        {
            text = Text;
            _type = InChat.All;
            mode = CommandParseMode.WithUsername;
        }

        public Command(string Text, CommandParseMode parseMode)
        {
            text = Text;
            _type = InChat.All;
            mode = parseMode;
        }

        public Command(InChat type, string Text, CommandParseMode parseMode)
        {
            _type = type;
            text = Text;
            mode = parseMode;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(param.IsCommand)
            {
                if(_type != InChat.All)
                    return param.InChat == _type && IsCommandEqual(param);

                return IsCommandEqual(param);
            }

            return false;
        }

        private bool IsCommandEqual(HandlerParams hParams)
        {
            switch(mode)
            {
                case CommandParseMode.WithUsername:
                    return string.Equals(text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase) && hParams.IsFullFormCommand;
                    
                case CommandParseMode.WithoutUsername:
                    return string.Equals(text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase) && !hParams.IsFullFormCommand;
                    
                case CommandParseMode.Both:
                    return string.Equals(text, hParams.CommandName, StringComparison.InvariantCultureIgnoreCase);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
