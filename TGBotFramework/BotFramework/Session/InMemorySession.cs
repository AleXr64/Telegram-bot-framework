using System.Collections.Concurrent;
using System.Collections.Generic;
using BotFramework.Abstractions.Storage;

namespace BotFramework.Session
{
    public class InMemorySession: IUserSession
    {
        private readonly ConcurrentDictionary<string, ISessionData> _sessions = new ConcurrentDictionary<string, ISessionData>();

        public InMemorySession(IBotUser user) { User = user; }

        public IBotUser User { get; }

        public IReadOnlyDictionary<string, ISessionData> SessionData => _sessions;

        public void AddOrUpdateData(ISessionData data) { _sessions.AddOrUpdate(data.Type, data, (key, old) => data); }

        public void RemoveData(ISessionData data)
        {
            if(_sessions.ContainsKey(data.Type))
            {
                _sessions.TryRemove(data.Type, out _);
            }
        }

        public void RemoveData(string type)
        {
            if(_sessions.ContainsKey(type))
            {
                _sessions.TryRemove(type, out _);
            }
        }
    }
}
