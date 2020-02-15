using System;
using BotFramework.Storage.EFCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotFramework.Storage.EFCore
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTelegramBotEFCoreContext<TContext>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction, 
            int poolSize = 128, bool trackUsers = true)
            where TContext: BotStorageContext
        {
            if(trackUsers)
            {
                services.AddSingleton(s => new StoreUsersService(s.GetService<IServiceScopeFactory>(), typeof(TContext)));
                services.AddTransient<IHostedService, StoreUsersService>(s => s.GetService<StoreUsersService>());
            }
            
            services.AddDbContextPool<TContext>(options =>
                {
                    options.UseLazyLoadingProxies();
                    optionsAction(options);
                });

            services.AddScoped<IBotFrameworkModule,Storage<TContext>>();
            return services;
        }

        public static IServiceCollection AddTeleramBotNewUsersHandler<THandler>(this IServiceCollection collection)
        where THandler: class, ITelegramBotNewUsersHandler
        {
            collection.AddScoped<ITelegramBotNewUsersHandler, THandler>();
            return collection;
        }
    }
}
