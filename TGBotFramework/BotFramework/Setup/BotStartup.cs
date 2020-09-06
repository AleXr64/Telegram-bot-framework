using System;
using System.Collections.Generic;
using BotFramework.Middleware;

namespace BotFramework.Setup
{
    public abstract class BotStartup
    {
        private readonly LinkedList<Type> _wares = new LinkedList<Type>();

        protected BotStartup(){}

        protected abstract void Setup();

        internal LinkedList<Type> __SetupInternal()
        {
            Setup();

            return _wares;
        }

        protected void UseMiddleware<T>() where T: IMiddleware { _wares.AddLast(typeof(T)); }
    }
}
