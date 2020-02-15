using Microsoft.EntityFrameworkCore;

namespace BotFramework.Storage.EFCore
{
    public abstract class BotStorageContext:DbContext
    {
        public BotStorageContext(DbContextOptions options):base(options)
        {
            
        }
        public virtual DbSet<TelegramChat> Chats { get; protected set; }
        public virtual DbSet<TelegramUser> Users { get; protected set; }
    }
}
