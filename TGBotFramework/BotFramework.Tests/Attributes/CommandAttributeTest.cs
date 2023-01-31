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
using BotFramework.Utils;

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
            var commandInChat = new ParametrizedCommandAttribute("test", CommandParseMode.Both);
            var command = new ParametrizedCommandAttribute("test", CommandParseMode.Both);

            var message = new Message
            {
                    Chat = new Chat() { Type = Telegram.Bot.Types.Enums.ChatType.Group },
                    Text = "/test@testbot",
                    Entities = new MessageEntity[]
                        {
                            new MessageEntity() { Length = 13, Offset = 0, Type = MessageEntityType.BotCommand }
                        }
                };
            var paramses = new HandlerParams(null, new Update 
                                                 { 
                                                     Message = message
                                                 },
                                             _serviceProvider, "testbot", _userProvider);

            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommandAttribute("test", CommandParseMode.WithUsername);
            command = new ParametrizedCommandAttribute("test", CommandParseMode.WithUsername);
            
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));

            commandInChat = new ParametrizedCommandAttribute("test", CommandParseMode.WithoutUsername);
            command = new ParametrizedCommandAttribute("test", CommandParseMode.WithoutUsername);

            Assert.False(commandInChat.CanHandleInternal(paramses));
            Assert.False(command.CanHandleInternal(paramses));

            message.Text = "/test";
            message.Entities = new MessageEntity[]
                {
                    new MessageEntity() { Length = 5, Offset = 0, Type = MessageEntityType.BotCommand }
                };
            paramses = new HandlerParams(null, new Update
                                                 {
                                                     Message = message
                                                 },
                                             _serviceProvider, "testbot", _userProvider);
            Assert.True(commandInChat.CanHandleInternal(paramses));
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInCaption()
        {
            var command = new ParametrizedCommandAttribute("test", TextContent.Caption);
            var paramses = new HandlerParams(null, new Update
            {
                                                     Message = new Message
                                                     {
                                                             Chat = new Chat() { Type = Telegram.Bot.Types.Enums.ChatType.Group },
                                                             Caption = "/test"
                                                            ,
                                                             CaptionEntities = new MessageEntity[]
                                                                 {
                                                                     new MessageEntity()
                                                                         {
                                                                             Length = 5,
                                                                             Offset = 0,
                                                                             Type = MessageEntityType.BotCommand
                                                                         }
                                                                 }

                                                         }
                                                 }, _serviceProvider,
                                             string.Empty, _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByText()
        {
            var command = new ParametrizedCommandAttribute("test");
            var paramses = new HandlerParams(null, new Update
            { Message = new Message
            {
                                                     Chat = new Chat() { Type = ChatType.Group },
                                                     Text = "/test"
                                                    ,
                                                     Entities = new MessageEntity[]
                                                         {
                                                             new MessageEntity()
                                                                 {
                                                                     Length = 5,
                                                                     Offset = 0,
                                                                     Type = MessageEntityType.BotCommand
                                                                 }
                                                         }
                                                 
                                                 } }, _serviceProvider,
                                             string.Empty, _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByTextWithUsername()
        {
            var command = new ParametrizedCommandAttribute("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot",
                                                         Entities = new MessageEntity[]
                                                             {
                                                                 new MessageEntity()
                                                                     {
                                                                         Length = 13,
                                                                         Offset = 0,
                                                                         Type = MessageEntityType.BotCommand
                                                                     }
                                                             }
                                                     }
                                                 },
                                             _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInChannel()
        {
            var command = new ParametrizedCommandAttribute("test");
            var update = new Update
            {
                    Message = new Message
                    {
                            Text = "/test@testbot",
                            Chat = new Chat { Type = ChatType.Channel },
                            Entities = new MessageEntity[]
                                {
                                    new MessageEntity()
                                        {
                                            Length = 5, Offset = 0, Type = MessageEntityType.BotCommand
                                        }
                                }
                        }
                };

            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Group;
            paramses = new HandlerParams(null,  update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Supergroup;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Private;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivateChat()
        {
            var command = new ParametrizedCommandAttribute("test");
            var update = new Update
            {
                    Message = new Message
                    {
                            Text = "/test@testbot",
                            Chat = new Chat { Type = ChatType.Channel },
                            Entities = new MessageEntity[]
                                {
                                    new MessageEntity()
                                        {
                                            Length = 5, Offset = 0, Type = MessageEntityType.BotCommand
                                        }
                                }
                        }
                };

            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Group;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Supergroup;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Private;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublicChat()
        {
            var command = new ParametrizedCommandAttribute("test");
            var update = new Update
            {
                    Message = new Message
                    {
                            Text = "/test@testbot",
                            Chat = new Chat { Type = Telegram.Bot.Types.Enums.ChatType.Channel },
                            Entities = new MessageEntity[]
                                {
                                    new MessageEntity()
                                        {
                                            Length = 5, Offset = 0, Type = MessageEntityType.BotCommand
                                        }
                                }
                        }
                };

            var paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Group;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Supergroup;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.True(command.CanHandleInternal(paramses));

            update.Message.Chat.Type = Telegram.Bot.Types.Enums.ChatType.Private;
            paramses = new HandlerParams(null, update, _serviceProvider, "testbot", _userProvider);
            Assert.False(command.CanHandleInternal(paramses));
        }
    }
}
