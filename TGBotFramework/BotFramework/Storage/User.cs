namespace BotFramework.Storage
{
    public class TelegramUser: TelegramStorageEntity
    {
        public virtual TelegramChat Chat { get; set; }
    }
}