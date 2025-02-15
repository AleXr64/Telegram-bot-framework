using System;
using System.Linq;

namespace BotFramework;

public sealed partial class HandlerParams
{
    public class Command
    {
        /// <summary>
        /// Short name without leading slash
        /// </summary>
        public string Name { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        /// <summary>
        /// Full command with slash and username if exists
        /// </summary>
        public string FullText { get; set; }
        /// <summary>
        /// Command contains username
        /// </summary>
        public bool IsFullCommand { get; set; }
    }
    public class ParametrizedCommand: Command
    {
        public ParametrizedCommand()
        {
            Offset = 0;
        }
        public string[] Args { get; set; }
    }
    
    internal static class CommandHelper
    {
        public static string GetCommand(string text, int offset, int length) => text.Substring(offset, length);


        public static bool IsMyCommand(string command, string username)
        {
            var name = command.Split('@').ElementAtOrDefault(1);
            return name?.Equals(username, StringComparison.InvariantCultureIgnoreCase) ?? true;
        }

        public static string[] GetCommandArgs(string text)
        {
            var args = text.Split(' ').Skip(1).ToArray();
            return args.Length > 0 ? args : [string.Empty];
        }

        public static string GetShortName(string text)
        {
            return (text.Split('@').FirstOrDefault() ?? text)[1..];
        }
    }
}
