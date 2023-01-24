using System;

namespace BotFramework.Attributes;

public enum ConditionType
{
    Any,
    All
}

[AttributeUsage(AttributeTargets.Method)]
public class HandleConditionAttribute : Attribute
{
    public ConditionType ConditionType { get; set; }
    public HandleConditionAttribute(ConditionType conditionType)
    {
        ConditionType = conditionType;
    }
}

