using System;

namespace BotFramework.ParameterResolvers
{
    public class DateTimeParameter: IParameterParser<DateTime>
    {
        public DateTime DefaultInstance() { return new DateTime(); }

        public bool TryGetValue(string text, out DateTime result) { return DateTime.TryParse(text, out result); }
    }

    public class DateTimeOffsetParameter: IParameterParser<DateTimeOffset>
    {
        public DateTimeOffset DefaultInstance() { return new DateTimeOffset(); }

        public bool TryGetValue(string text, out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(text, out result);
        }
    }

    public class TimeSpanParameter: IParameterParser<TimeSpan>
    {
        public TimeSpan DefaultInstance() { return new TimeSpan(); }

        public bool TryGetValue(string text, out TimeSpan result) { return TimeSpan.TryParse(text, out result); }
    }
}
