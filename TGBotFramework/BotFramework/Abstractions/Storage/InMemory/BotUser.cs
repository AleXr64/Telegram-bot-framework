using System;

namespace BotFramework.Abstractions.Storage.InMemory
{
    class BotUser:IBotUser
    {

        public long TelegramId { get; set; }

        public IBotChat Chat { get; set; }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
        return TelegramId.GetHashCode() * Chat.TelegramId.GetHashCode();
#else
        return HashCode.Combine(TelegramId.GetHashCode(), Chat.TelegramId.GetHashCode());
#endif
        }

        public override bool Equals(object? obj)
        {
            if(obj is BotUser another)
            {
                return TelegramId == another.TelegramId && Chat.TelegramId == another.Chat.TelegramId;
            }

            return  base.Equals(obj);
        }
    }
}
