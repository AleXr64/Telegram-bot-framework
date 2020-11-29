using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    internal sealed class Router: BaseMiddleware
    {
        private readonly EventHandlerFactory _factory;

        internal Router(EventHandlerFactory factory) { _factory = factory; }

        protected override async Task Process() { await _factory.ExecuteHandler(HandlerParams); }
    }
}
