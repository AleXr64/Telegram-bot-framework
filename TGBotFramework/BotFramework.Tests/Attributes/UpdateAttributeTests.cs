using System;
using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Enums;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;


namespace BotFramework.Tests.Attributes
{
    public class UpdateAttributeTests
    {
        private static IUserProvider _userProvider = new DefaultUserProvider();
        private static IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

        [Fact]
        public void CanHandleFlags()
        {
            var paramses = new HandlerParams(null, new Update {Message = new Message {Text = "Blah"}},
                                             _serviceProvider, "testbot", _userProvider);

            var updateAttr = new UpdateAttribute { UpdateFlags = UpdateFlag.Message | UpdateFlag.Poll};

            Assert.True(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { InlineQuery = new InlineQuery()}, _serviceProvider, "test", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { ChosenInlineResult = new ChosenInlineResult()}, _serviceProvider, "test", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Poll = new Poll()}, _serviceProvider, "test", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            updateAttr = new UpdateAttribute { UpdateFlags = UpdateFlag.All };
            paramses = new HandlerParams(null, new Update(), _serviceProvider, "test", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }
        
    }
}
