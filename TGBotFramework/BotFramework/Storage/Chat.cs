using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotFramework.Storage
{
    public class TelegramChat: TelegramStorageEntity
    {
        [InverseProperty("Chat")]
        public virtual ICollection<TelegramUser> Users { get; set; }
    }
}
