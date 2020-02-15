using System;
using System.Threading.Tasks;
using BotFramework.Storage.EFCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Storage.EFCore
{
    public class Storage<TContext>:IBotFrameworkModule where TContext: BotStorageContext
    {
        private readonly StoreUsersService _service;

        public Storage(TContext context, IServiceProvider service)
        {
            DbContext = context;
            _service = service.GetService<StoreUsersService>();
        }

        async Task IBotFrameworkModule.PreHandler(HandlerParams handlerParams)
        {
            if(handlerParams.HasChat && handlerParams.HasFrom)
            {
                var chat = new TelegramChat();
                chat.RealId = handlerParams.Chat.Id;
                var user = new TelegramUser();
                user.Chat = chat;
                user.RealId = handlerParams.From.Id;
                await _service.AddToQueue(user);
            }
        }
        public TContext DbContext { get; }
    }

    public interface ITelegramBotNewUsersHandler
    {
        Task SaveAction(TelegramUser user, TelegramChat chat);
    }

}
