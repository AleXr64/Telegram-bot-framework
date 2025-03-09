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
}
