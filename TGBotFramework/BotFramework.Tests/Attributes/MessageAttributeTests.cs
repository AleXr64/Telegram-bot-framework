using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Enums;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Xunit;
using Message=Telegram.Bot.Types.Message;

namespace BotFramework.Tests.Attributes
{
    public class MessageAttributeTests
    {
        private static readonly IUserProvider _userProvider = new DefaultUserProvider();

        [Fact]
        public void CanHandleVoiceMessages()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Voice = new Voice() } },
                                             null, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasVoice);

            Assert.True(attribute.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Message = new Message { Animation = new Animation() } },
                                         null, "testbot", _userProvider);

            Assert.False(attribute.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleMultiContent()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Voice = new Voice(), Game = new Game() } },
                                             null, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasVoice);

            Assert.True(attribute.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Poll = new Poll() },
                                         null, "testbot", _userProvider);

            Assert.False(attribute.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleCaptionMessages()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Caption = "Blah", Voice = new Voice() } },
                                             null, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasCaption);

            Assert.True(attribute.CanHandleInternal(paramses));

            attribute = new MessageAttribute("Foo");
            Assert.False(attribute.CanHandleInternal(paramses));

            attribute = new MessageAttribute("/test");
            Assert.False(attribute.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleNewChatMembers()
        {
            var handles = new HandlerParams(null, new Update()
                {
                    Message = new Message()
                        {
                            NewChatMembers = new User[]
                                {
                                    new User()
                                        {
                                            Id = 12345,
                                            FirstName = "Fulan",
                                            Username = "fulan",
                                            LastName = "Bin Fulan"
                                        }
                                }
                        }
                }, null, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasNewChatMembers);
            Assert.True(attribute.CanHandleInternal(handles));

        }

        [Fact]
        public void CanHandleLeftChatMember()
        {
            var handles = new HandlerParams(null, new Update()
                {
                    Message = new Message()
                        {
                            LeftChatMember =
                                new User()
                                    {
                                        Id = 12345,
                                        FirstName = "Fulan",
                                        Username = "fulan",
                                        LastName = "Bin Fulan"
                                    }

                        }
                }, null, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasLeftChatMember);
            Assert.True(attribute.CanHandleInternal(handles));

        }

    }
}
