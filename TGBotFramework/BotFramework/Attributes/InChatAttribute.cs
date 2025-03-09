using BotFramework.Enums;
using System;

namespace BotFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InChatAttribute : Attribute
    {
        public InChat ChatType { get; set; }
        public InChatAttribute(InChat chatType = InChat.All)
        {
            ChatType = chatType;
        }
    }
}
