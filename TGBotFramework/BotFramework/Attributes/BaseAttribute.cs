using System;

namespace BotFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class HandlerAttribute: Attribute
    {
        protected abstract bool CanHandle(HandlerParams param);
        internal bool CanHandleInternal(HandlerParams param) => CanHandle(param);

        public override object TypeId => Guid.NewGuid();
    }
}
