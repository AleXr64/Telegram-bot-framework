using System.Globalization;

namespace BotFramework.ParameterResolvers
{
    public class IntParameter: IParameterParser<int>
    {
        public int DefaultInstance() { return 0; }

        public bool TryGetValue(string text, out int result) { return int.TryParse(text, out result); }
    }

    public class LongParameter: IParameterParser<long>
    {
        public long DefaultInstance() { return 0; }

        public bool TryGetValue(string text, out long result) { return long.TryParse(text, out result); }
    }

    public class FloatParameter: IParameterParser<float>
    {
        public float DefaultInstance() { return new float(); }

        public bool TryGetValue(string text, out float result)
        {
            return float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }

    public class DecimalParameter: IParameterParser<decimal>
    {
        public decimal DefaultInstance() { return new decimal(); }

        public bool TryGetValue(string text, out decimal result) { return decimal.TryParse(text, out result); }
    }

    public class DoubleParameter: IParameterParser<double>
    {
        public double DefaultInstance() { return new double(); }

        public bool TryGetValue(string text, out double result) { return double.TryParse(text, out result); }
    }
}
