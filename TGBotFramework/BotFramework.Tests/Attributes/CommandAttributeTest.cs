using BotFramework.Attributes;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace BotFramework.Tests.Attributes
{
    public class CommandAttributeTest
    {
        [Fact]
        public void CanFilterUserName()
        {
            var command = new Command("test", CommandParseMode.Both);
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));

            command = new Command("test", CommandParseMode.WithUsername);
            Assert.True(command.CanHandleInternal(paramses));

            command = new Command("test", CommandParseMode.WithoutUsername);
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } },
                                         null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByText()
        {
            var command = new Command("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test" } }, null,
                                             string.Empty);
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleByTextWithUsername()
        {
            var command = new Command("test");
            var paramses = new HandlerParams(null, new Update { Message = new Message { Text = "/test@testbot" } },
                                             null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInChannel()
        {
            var command = new Command(InChat.Channel, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Channel }
                                      }
                                  }, null, "testbot");
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
                                  }, null, "testbot");
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
                                  }, null, "testbot");
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
                                  }, null, "testbot");
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPrivateChat()
        {
            var command = new Command(InChat.Private, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, null, "testbot");
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, null, "testbot");
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, null, "testbot");
            Assert.False(command.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleInPublicChat()
        {
            var command = new Command(InChat.Public, "test");
            var paramses =
                new HandlerParams(null,
                                  new Update
                                  {
                                      Message = new Message
                                      {
                                          Text = "/test@testbot",
                                          Chat = new Chat { Type = ChatType.Private }
                                      }
                                  }, null, "testbot");
            Assert.False(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Group }
                                             }
                                         }, null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Supergroup }
                                             }
                                         }, null, "testbot");
            Assert.True(command.CanHandleInternal(paramses));

            paramses = new HandlerParams(null,
                                         new Update
                                         {
                                             Message = new Message
                                             {
                                                 Text = "/test@testbot",
                                                 Chat = new Chat { Type = ChatType.Channel }
                                             }
                                         }, null, "testbot");

            Assert.False(command.CanHandleInternal(paramses));
        }
    }
}
