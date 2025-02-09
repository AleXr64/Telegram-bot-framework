using BotFramework.Enums;
using System.Text.RegularExpressions;

namespace BotFramework.Attributes
{
    public class RegexTextMessageAttribute: TextMessageAttribute
    {
        internal override bool IsRegex => true;
        internal RegexOptions RegexOptions { get; set; } = RegexOptions.None;
        public RegexTextMessageAttribute(string regexPattern)
            : base(regexPattern) {  }

        public RegexTextMessageAttribute(string regexPattern, TextContent textContent)
            : this(regexPattern)
        {
            TextContent = textContent;
        }
        public RegexTextMessageAttribute(string regexPattern, TextContent textContent, RegexOptions regexOptions)
            : this(regexPattern, textContent)
        {
            RegexOptions = regexOptions;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            return base.CanHandle(param) && Regex.IsMatch(MessageText, Text, RegexOptions);
        }
    }
}
