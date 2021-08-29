namespace BotFramework.Abstractions.Storage
{
    public interface IUserProvider
    {
        IBotUser GetUser(long userId, long chatId);
    }
}
