using System;

namespace BotFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PriorityAttribute : Attribute
    {
        public short Value { get; set; } = 0;
        public PriorityAttribute(short priority)
        {
            Value = priority;
        }
    }
}
