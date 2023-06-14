using BotFramework.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BotFramework.Attributes
{
    public class RegexTextMessageAttribute: TextMessageAttribute
    {
        internal RegexOptions RegexOptions { get; set; } = RegexOptions.None;

        public RegexTextMessageAttribute(string regex_pattern)
            : base(regex_pattern) { }

        public RegexTextMessageAttribute(string regex_pattern, TextContent textContent)
            : this(regex_pattern)
        {
            TextContent = textContent;
        }
        public RegexTextMessageAttribute(string regex_pattern, TextContent textContent, RegexOptions regexOptions)
            : this(regex_pattern, textContent)
        {
            IsRegex = true;
            RegexOptions = regexOptions;
        }

        protected override bool CanHandle(HandlerParams param)
        {
            if(!base.CanHandle(param))
            {
                return false;
            }

            return Regex.IsMatch(MessageText, Text, RegexOptions);

        }
    }
}
