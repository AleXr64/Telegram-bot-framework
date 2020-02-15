namespace BotFramework.Setup
{
    /// <summary>
    ///     Chat Type
    /// </summary>
    public enum InChat
    {
        /// <summary>
        ///     Private chat aka peer-to-peer with bot
        /// </summary>
        Private,

        /// <summary>
        ///     Public chats (in both privacy mode), not peer-to-peer with bot
        /// </summary>
        Public,
        Channel,
        All
    }
}
