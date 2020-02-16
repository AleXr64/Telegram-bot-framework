using System;
using Xunit;

namespace BotFramework.Tests
{
    public class CommandHelperTests
    {
        [Fact]
        public void IsCommand()
        {
            var text = "/test@testbot";
            Assert.True(text.IsCommand());
            Assert.True(text.IsCommand("testbot"));
            Assert.False(text.IsCommand("anotherbot"));
            Assert.False(string.Empty.IsCommand());
        }

        [Fact]
        public void GetCommandName()
        {
            var fullForm = "/test@testbot";
            var shortForm = "/test";
            var command = "test";
            Assert.NotEqual(command, fullForm.GetCommandName());
            Assert.Equal(command, fullForm.GetCommandName("testbot"));

            Assert.Equal(command, shortForm.GetCommandName());

            Assert.Equal(string.Empty, string.Empty.GetCommandName());
            Assert.Equal(string.Empty, string.Empty.GetCommandName("testbot"));

            Assert.NotEqual(command, fullForm.GetCommandName("anotherbot"));

            Assert.Equal(string.Empty, "blabla".GetCommandName());
            Assert.Equal(string.Empty, "blabla".GetCommandName("bot"));
        }

        [Fact]
        public void GetCommandArgs()
        {
            var fullForm = "/test@testbot bla foo";
            var shortForm = "/test bla foo";
            var args = new[] { "bla", "foo" };

            Assert.Equal(args, shortForm.GetCommandArgs());
            Assert.Equal(args, fullForm.GetCommandArgs());

            Assert.Equal(args, shortForm.GetCommandArgs());
            Assert.Equal(args, shortForm.GetCommandArgs());
        }
    }
}
