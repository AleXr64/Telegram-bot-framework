using System;

namespace BotFramework.Enums
{
    /// <summary>
    ///     Chat Type
    /// </summary>
    [Flags]
    public enum InChat
    {
        /// <summary>
        ///     Private chat aka peer-to-peer with bot
        /// </summary>
        Private = 1 << 0,

        /// <summary>
        ///     Public chats (in both privacy mode), not peer-to-peer with bot
        /// </summary>
        Public = 1 << 1,
        Channel = 1 << 2,
        All = Private | Public | Channel
    }
}
