using System;
using System.ComponentModel.DataAnnotations;

namespace BotFramework.Storage
{
    public abstract class StorageEntity
    {
        [Key]
        public Guid Id { get; set; }

    }

    public abstract class TelegramStorageEntity: StorageEntity
    {
        /// <summary>
        /// Any "Real" (not key!) identifier from Telegram
        /// </summary>
        public long RealId { get; set; }
    }
}
