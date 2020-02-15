namespace BotFramework.ParameterResolvers
{
    public class StringParametr: IParameterParser<string>
    {
        public string DefaultInstance() { return string.Empty; }

        public bool TryGetValue(string text, out string result)
        {
            result = text;
            return true;
        }
    }
}
