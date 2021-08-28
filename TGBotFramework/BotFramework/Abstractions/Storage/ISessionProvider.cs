using BotFramework.Abstractions.Storage;

namespace BotFramework.Abstractions
{
    public interface ISessionProvider
    {
        IUserSession GetOrCreateSession(IBotUser user);

        void SaveSession(IUserSession session);

        void DestroySession(IUserSession session);
    }
}
