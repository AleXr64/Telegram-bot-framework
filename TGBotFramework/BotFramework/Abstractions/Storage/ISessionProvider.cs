namespace BotFramework.Abstractions.Storage
{
    public interface ISessionProvider
    {
        IUserSession GetOrCreateSession(IBotUser user);

        void SaveSession(IUserSession session);

        void DestroySession(IUserSession session);
    }
}
