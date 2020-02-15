namespace BotFramework.ParameterResolvers
{
    public class BoolParameter: IParameterParser<bool>
    {
        public bool DefaultInstance() { return false; }

        public bool TryGetValue(string text, out bool result) { return bool.TryParse(text, out result); }
    }
}
