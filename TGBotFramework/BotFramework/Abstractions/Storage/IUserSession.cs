using System.Collections.Generic;

namespace BotFramework.Abstractions.Storage
{
    public interface IUserSession
    {
        IBotUser User { get; }

        IReadOnlyDictionary<string, ISessionData> SessionData {  get; }

        void AddOrUpdateData(ISessionData data);

        void RemoveData(ISessionData data);

        void RemoveData(string type);
    }

    public interface ISessionData
    {
        string Type { get; }
    }

    public interface ISessionData<out TData>: ISessionData where TData: class, new()
    {
        TData Data { get; }
    }
}
