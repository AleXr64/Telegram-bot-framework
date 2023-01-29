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

        [Fact]
        public void CanHandleInChannel()
        {
            var updateAttr = new UpdateAttribute { InChatFlags = Enums.InChat.Channel };

            var chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivate()
        {
            var updateAttr = new UpdateAttribute { InChatFlags = Enums.InChat.Private };

            var chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublic()
        {
            var updateAttr = new UpdateAttribute { InChatFlags = Enums.InChat.Public };

            var chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublicAndPrivate()
        {
            var updateAttr = new UpdateAttribute { InChatFlags = Enums.InChat.Public | Enums.InChat.Private };

            var chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel };
            var update = new Update { Message = new Message { Text = "Blah", Chat = chat } };

            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Group };
            update = new Update { Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Private };
            update = new Update { Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Supergroup };
            update = new Update { Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleAll()
        {
            var updateAttr = new UpdateAttribute { InChatFlags = Enums.InChat.All };

            var chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }
    }
}
