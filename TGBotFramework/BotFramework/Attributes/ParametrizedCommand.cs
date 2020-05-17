using BotFramework.Setup;

namespace BotFramework.Attributes
{
    public class ParametrizedCommand: Command
    {
        public ParametrizedCommand(string text)
            : base(text) { }

        public ParametrizedCommand(InChat type, string text)
            : base(type, text) { }

        public ParametrizedCommand(string text, CommandParseMode parseMode)
            : base(text, parseMode) { }

        public ParametrizedCommand(InChat type, string text, CommandParseMode parseMode)
            : base(type, text, parseMode) { }
    }
}
