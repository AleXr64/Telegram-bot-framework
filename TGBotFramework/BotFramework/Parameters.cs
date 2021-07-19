using System.Linq;
using Telegram.Bot.Types;

namespace BotFramework
{
    public interface IParameterParser<T>
    {
        T DefaultInstance();
        bool TryGetValue(string text, out T result);
    }

    public interface IRawParameterParser<T>
    {
        T DefaultInstance();
        bool TryGetValueByRawUpdate(Update update, out T result);
    }

    public sealed class CommandParameter
    {
        internal CommandParameter(int position,  object typed)
        {
            Position = position;
            TypedValue = typed;
        }

        public int Position { get; }
        public object TypedValue { get; }
    }

    internal static class CommandHelper
    {
        public static bool IsCommand(this string text, string me = "")
        {
            if(!string.IsNullOrEmpty(me) && text.StartsWith('/'))
            {
                var str = text.Substring(1).Split(" ")[0];
                return text.StartsWith("/") && str.EndsWith('@' + me);
            }
            return text.StartsWith("/");
        }

        public static string GetCommandName(this string str, string me = "")
        {
            var text = str[1..];
            if(!string.IsNullOrEmpty(me))
            {
               return text.Split(" ")[0].ToLowerInvariant().Replace('@' + me.ToLowerInvariant(), "");
            }
            return text.Split(" ")[0].ToLowerInvariant();
        }

        public static string[] GetCommandArgs(this string text)
        {
            return text.Split(" ").Skip(1).ToArray();
        }
    }
}
