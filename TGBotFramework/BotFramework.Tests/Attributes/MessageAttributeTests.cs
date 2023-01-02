using System;
using BotFramework.Abstractions.Storage;
using BotFramework.Attributes;
using BotFramework.Enums;
using BotFramework.Setup;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Xunit;
using Message = Telegram.Bot.Types.Message;

namespace BotFramework.Tests.Attributes
{
    public class MessageAttributeTests
    {
        private static readonly IUserProvider _userProvider = new DefaultUserProvider();
        private static IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

        [Fact]
        public void CanHandleVoiceMessages()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Voice = new Voice() } },
                                             _serviceProvider, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasVoice);

            Assert.True(attribute.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Message = new Message { Animation = new Animation() } },
                                         _serviceProvider, "testbot", _userProvider);

            Assert.False(attribute.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleMultiContent()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Voice = new Voice(), Game = new Game() } },
                                             _serviceProvider, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasVoice);

            Assert.True(attribute.CanHandleInternal(paramses));

            paramses = new HandlerParams(null, new Update { Poll = new Poll() },
                                         _serviceProvider, "testbot", _userProvider);

            Assert.False(attribute.CanHandleInternal(paramses));
        }

        [Fact]
        public void CanHandleCaptionMessages()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Caption = "Blah", Voice = new Voice() } },
                                             _serviceProvider, "testbot", _userProvider);

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
                }, _serviceProvider, "testbot", _userProvider);

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
                }, _serviceProvider, "testbot", _userProvider);

            var attribute = new MessageAttribute(MessageFlag.HasLeftChatMember);
            Assert.True(attribute.CanHandleInternal(handles));
        }

        [Fact]
        public void CanHandleVideoChatTest()
        {
            var handles = new HandlerParams(
                null,
                new Update()
                    {
                        Message = new Message()
                            {
                                VideoChatScheduled = new VideoChatScheduled() { },
                                VideoChatStarted = new VideoChatStarted() { },
                                VideoChatEnded = new VideoChatEnded() { },
                                VideoChatParticipantsInvited = new VideoChatParticipantsInvited() { }
                            }
                    }, _serviceProvider, "testbot", _userProvider);

            var videoScheduledAttribute = new MessageAttribute(MessageFlag.HasVideoChatScheduled);
            var videoStartedAttribute = new MessageAttribute(MessageFlag.HasVideoChatStarted);
            var videoEndedAttribute = new MessageAttribute(MessageFlag.HasVideoChatEnded);
            var videoInvitedAttribute = new MessageAttribute(MessageFlag.HasVideoChatParticipantsInvited);

            Assert.True(videoScheduledAttribute.CanHandleInternal(handles));
            Assert.True(videoStartedAttribute.CanHandleInternal(handles));
            Assert.True(videoEndedAttribute.CanHandleInternal(handles));
            Assert.True(videoInvitedAttribute.CanHandleInternal(handles));
        }
    }
}
