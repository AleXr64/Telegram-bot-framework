namespace BotFramework.Setup
{
    /// <summary>
    ///     Consider the user name in the command or not
    /// </summary>
    public enum CommandParseMode
    {
        /// <summary>
        ///     [Default behavior!] Allow trigger only on full command form (i.e. /start@usernamebot)
        /// </summary>
        WithUsername,

        /// <summary>
        ///     Allow trigger only on short command form (i.e. /start)
        /// </summary>
        WithoutUsername,

        /// <summary>
        ///     Trigger command on any form
        /// </summary>
        Both
    }
}
