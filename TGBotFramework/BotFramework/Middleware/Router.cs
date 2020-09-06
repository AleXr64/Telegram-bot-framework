using System;
using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    public sealed class Router : BaseMiddleware
    {
        private EventHandlerFactory _factory;

        internal Router(EventHandlerFactory factory) { _factory = factory; }

        /// <summary>
        /// This class SHOULD NOT been instanced manually!
        /// </summary>
        public Router()
        {
            throw new AccessViolationException("This class SHOULD NOT been instanced manually!");
        }

        protected override async Task Process()
        {
            await _factory.ExecuteHandler(HandlerParams);
        }
    }
}
