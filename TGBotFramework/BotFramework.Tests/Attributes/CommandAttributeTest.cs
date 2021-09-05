﻿using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Session;
using BotFramework.Setup;
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
        [Fact]
        public void CanFilterUserName()
        {
            var commandInChat = new ParametrizedCommand(InChat.All,"test", CommandParseMode.Both);
            var command = new ParametrizedCommand("test", CommandParseMode.Both);
            
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             null, "testbot", _userProvider);

            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommand(InChat.All,"test", CommandParseMode.WithUsername);
            command = new ParametrizedCommand("test", CommandParseMode.WithUsername);
            
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommand(InChat.All,"test", CommandParseMode.WithoutUsername);
            command = new ParametrizedCommand("test", CommandParseMode.WithoutUsername);

            Assert.False(commandInChat.CanHandleInternal(paramses));
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } },
                                         null, "testbot", _userProvider);
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByText()
        {
            var command = new ParametrizedCommand("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } }, null,
                                             string.Empty, _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByTextWithUsername()
        {
            var command = new ParametrizedCommand("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             null, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInChannel()
        {
            var command = new ParametrizedCommand(InChat.Channel, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Channel }
                                      }
                                  }, null, "testbot", _userProvider);
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
                                  }, null, "testbot", _userProvider);
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
                                  }, null, "testbot", _userProvider);
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
                                  }, null, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivateChat()
        {
            var command = new ParametrizedCommand(InChat.Private, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, null, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, null, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, null, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, null, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublicChat()
        {
            var command = new ParametrizedCommand(InChat.Public, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, null, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, null, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, null, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, null, "testbot", _userProvider);

            Assert.False(command.CanHandleInternal(paramses));
        }
    }
}
