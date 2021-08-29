using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;


namespace BotFramework.Tests.Attributes
{
    public class UpdateAttributeTests
    {
        private static IUserProvider _userProvider = new DefaultUserProvider();

        [Fact]
        public void CanHandleFlags()
        {
            var paramses = new HandlerParams(null, new Update {Message = new Message {Text = "Blah"}},
                                             null, "testbot", _userProvider);

            var updateAttr = new UpdateAttribute { UpdateFlags = UpdateFlag.Message | UpdateFlag.Poll};

            Assert.True(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { InlineQuery = new InlineQuery()}, null, "test", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { ChosenInlineResult = new ChosenInlineResult()}, null, "test", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Poll = new Poll()}, null, "test", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            updateAttr = new UpdateAttribute { UpdateFlags = UpdateFlag.All };
            paramses = new HandlerParams(null, new Update(), null, "test", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInChannel()
        {
            var updateAttr = new UpdateAttribute { InChat = Setup.InChat.Channel };

            var chat = new Chat { Type = ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivate()
        {
            var updateAttr = new UpdateAttribute { InChat = Setup.InChat.Private };

            var chat = new Chat { Type = ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublic()
        {
            var updateAttr = new UpdateAttribute { InChat = Setup.InChat.Public };

            var chat = new Chat { Type = ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.False(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleAll()
        {
            var updateAttr = new UpdateAttribute { InChat = Setup.InChat.All };

            var chat = new Chat { Type = ChatType.Channel };
            var update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            
            var paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Group};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Private};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));

            chat = new Chat { Type = ChatType.Supergroup};
            update  = new Update {Message = new Message { Text = "Blah", Chat = chat } };
            paramses = new HandlerParams(null, update, null, "testbot", _userProvider);
            Assert.True(updateAttr.CanHandleInternal(paramses));
        }
    }
}
