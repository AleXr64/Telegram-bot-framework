using BotFramework.Setup;

namespace BotFramework.Attributes
{
    public class ParametrizedCommand: Command
    {
        public ParametrizedCommand(string Text)
            : base(Text) { }

        public ParametrizedCommand(InChat type, string Text)
            : base(type, Text) { }

        public ParametrizedCommand(string Text, CommandParseMode parseMode)
            : base(Text, parseMode) { }

        public ParametrizedCommand(InChat type, string Text, CommandParseMode parseMode):base(type,Text, parseMode)
        {
            
        }
    }
}
