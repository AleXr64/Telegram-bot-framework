using System;
using System.Collections.Concurrent;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;

namespace BotFramework.Session
{
    public class InMemorySessionProvider: ISessionProvider
    {
        private readonly ConcurrentDictionary<IBotUser, IUserSession> _userSessions = new();

        public IUserSession GetOrCreateSession(IBotUser user)
        {
            return _userSessions.GetOrAdd(user, new InMemorySession(user));
        }

        public void SaveSession(IUserSession session) => _userSessions.AddOrUpdate(session.User, session, (user, userSession) => session);

        public void DestroySession(IUserSession session) => _userSessions.TryRemove(session.User, out _);
    }
}
