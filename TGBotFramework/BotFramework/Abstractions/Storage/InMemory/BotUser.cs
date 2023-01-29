using System;

namespace BotFramework.Abstractions.Storage.InMemory
{
    class BotUser:IBotUser
    {

        public long TelegramId { get; set; }

        public IBotChat Chat { get; set; }

        public override int GetHashCode() => HashCode.Combine(TelegramId.GetHashCode(), Chat.TelegramId.GetHashCode());

        public override bool Equals(object obj)
        {
            if(obj is BotUser another)
            {
                return TelegramId == another.TelegramId && Chat.TelegramId == another.Chat.TelegramId;
            }

            return  base.Equals(obj);
        }
    }
}
