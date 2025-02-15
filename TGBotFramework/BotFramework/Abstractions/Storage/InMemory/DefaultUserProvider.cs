namespace BotFramework.Abstractions.Storage.InMemory
{
    internal class DefaultUserProvider: IUserProvider
    {

        public IBotUser GetUser(long userId, long chatId)
        {
            var chat = new BotChat { TelegramId = chatId };
            var user = new BotUser { Chat = chat, TelegramId = userId };

            return user;
        }
    }
}
