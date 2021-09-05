namespace BotFramework.Abstractions.Storage.InMemory
{
    class BotChat : IBotChat
    {
        public long TelegramId { get; set; }
    }
}
