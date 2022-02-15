using System;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Enums;
using BotFramework.Session;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;
using Message = Telegram.Bot.Types.Message;

namespace BotFramework.Tests.Attributes
{
    public class CommandAttributeTest
    {
        private static IUserProvider _userProvider = new DefaultUserProvider();

        private static ISessionProvider _sessionProvider = new InMemorySessionProvider();
        private static IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();
        [Fact]
        public void CanFilterUserName()
        {
            var commandInChat = new ParametrizedCommandAttribute(InChat.All,"test", CommandParseMode.Both);
            var command = new ParametrizedCommandAttribute("test", CommandParseMode.Both);
            
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             _serviceProvider, "testbot", _userProvider);

            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommandAttribute(InChat.All,"test", CommandParseMode.WithUsername);
            command = new ParametrizedCommandAttribute("test", CommandParseMode.WithUsername);
            
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommandAttribute(InChat.All,"test", CommandParseMode.WithoutUsername);
            command = new ParametrizedCommandAttribute("test", CommandParseMode.WithoutUsername);

            Assert.False(commandInChat.CanHandleInternal(paramses));
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } },
                                         _serviceProvider, "testbot", _userProvider);
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByText()
        {
            var command = new ParametrizedCommandAttribute("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } }, _serviceProvider,
                                             string.Empty, _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByTextWithUsername()
        {
            var command = new ParametrizedCommandAttribute("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInChannel()
        {
            var command = new ParametrizedCommandAttribute(InChat.Channel, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Channel }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Group }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Supergroup }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivateChat()
        {
            var command = new ParametrizedCommandAttribute(InChat.Private, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublicChat()
        {
            var command = new ParametrizedCommandAttribute(InChat.Public, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, _serviceProvider, "testbot", _userProvider);

            Assert.False(command.CanHandleInternal(paramses));
        }
    }
}
