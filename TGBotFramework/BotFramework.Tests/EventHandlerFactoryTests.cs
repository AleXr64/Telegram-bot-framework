using BotFramework.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Xunit;

namespace BotFramework.Tests
{
    public class EventHandlerFactoryTests
    {
        private EventHandlerFactory _factory;
        private ServiceCollection _services;
        private TestProxy _proxy;

        private void Setup()
        {
            _services = new ServiceCollection();
            _proxy = new TestProxy();
            _services.AddSingleton(s => _proxy);
            var scopeFactory = _services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            _factory = new EventHandlerFactory(scopeFactory);
        }

        private class TestProxy
        {
            public bool Acessed;
            public bool Executed;
        }

        private class TestHandlerAttribute: HandlerAttribute
        {
            protected override bool CanHandle(HandlerParams param) { return true; }
        }

        private class TestHandler: BotEventHandler
        {
            private readonly TestProxy _proxy;

            public TestHandler(TestProxy proxy)
            {
                _proxy = proxy;
                _proxy.Acessed = true;
            }

            [TestHandler]
            public void Fact() { _proxy.Executed = true; }
        }

        [Fact]
        public async Task Test()
        {
            Setup();
            _factory.Find();
            var hParams = new HandlerParams(null, new Update(), _services.BuildServiceProvider(), "test");
            await _factory.ExecuteHandler(hParams);
            Assert.True(_proxy.Acessed);
            Assert.True(_proxy.Executed);
        }
    }
}
