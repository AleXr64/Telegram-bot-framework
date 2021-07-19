using BotFramework.Attributes;
using BotFramework.Setup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;
using Message = Telegram.Bot.Types.Message;

namespace BotFramework.Tests.Attributes
{
    public class MessageAttributeTests
    {
        [Fact]
        public void CanHandleVoiceMessages()
        {
            var paramses = new HandlerParams(null, new Update { Message = new Message { Voice = new Voice { } } }, null, "testbot" );
            var attribute = new MessageAttribute(Enums.MessageFlag.HasVoice);

            Assert.True(attribute.CanHandleInternal(paramses));
            
            paramses = new HandlerParams(null, new Update(){Message = new Message(){Animation = new Animation()}}, null, "testbot");

            Assert.False(attribute.CanHandleInternal(paramses));

        }

        [Fact]
        public void CanHandleCaptionMessages()
        {
            var paramses = new HandlerParams(null, new Update() { Message = new Message() { Caption = "Blah", Voice = new Voice { } } }, null, "testbot");
            var attribute = new MessageAttribute(Enums.MessageFlag.HasCaption);

            Assert.True(attribute.CanHandleInternal(paramses));

            attribute = new MessageAttribute("Foo");
            Assert.False(attribute.CanHandleInternal(paramses));

            attribute = new MessageAttribute("/test");
            Assert.False(attribute.CanHandleInternal(paramses));
        }

    }
}
