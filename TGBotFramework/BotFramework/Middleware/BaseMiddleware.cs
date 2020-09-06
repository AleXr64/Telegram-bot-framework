using System;
using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    public abstract class BaseMiddleware: IMiddleware
    {
        private IMiddleware _next;
        protected HandlerParams HandlerParams { get; private set; }

        protected abstract Task Process();

        internal async Task __ProcessInternal()
        {
            await Process();
        }

        protected void Next()
        {
            ((BaseMiddleware)_next).__ProcessInternal().Wait();
        }

        internal void __Setup(IMiddleware ware, HandlerParams handlerParams)
        {
            _next = ware;
            HandlerParams = handlerParams;
        }
    }
}
