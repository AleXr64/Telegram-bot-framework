﻿using System;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Middleware;
using BotFramework.ParameterResolvers;
using BotFramework.Session;
using BotFramework.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotFramework
{
    public static class ServiceExtensions
    {
        public static void AddTelegramBot(this IServiceCollection collection)
        {
            collection.AddDefaultParameterParsers();

            collection.AddSingleton<IBotInstance, Bot>();
            collection.AddTransient<IHostedService>(x => (Bot)x.GetService<IBotInstance>());
            
            //won't break existent 0.5 users
            collection.AddTelegramBotInMemoryStorage();
        }

        public static void AddTelegramBot<T>(this IServiceCollection collection) where T: BotStartup, new()
        {
            collection.AddDefaultParameterParsers();

            var startup = new T();

            var wares = startup.__SetupInternal();

            foreach(var ware in wares)
            {
                collection.AddScoped(typeof(IMiddleware),ware);
            }

            collection.AddSingleton<IBotInstance, Bot>(x => new Bot(x, x.GetRequiredService<IServiceScopeFactory>(), typeof(T)));
            collection.AddTransient<IHostedService>(x => (Bot)x.GetService<IBotInstance>());

            //won't break existent 0.5 users
            collection.AddTelegramBotInMemoryStorage();
        }

        public static IServiceCollection AddTelegramBotParameterParser<TParam, TParser>(
            this IServiceCollection collection)
            where TParser : class, IParameterParser<TParam>
        {
            collection.AddScoped<IParameterParser<TParam>, TParser>();
            return collection;
        }
        public static IServiceCollection AddTelegramBotRawUpdateParser<TParam, TParser>(
            this IServiceCollection collection)
            where TParser : class, IRawParameterParser<TParam>
        {
            collection.AddScoped<IRawParameterParser<TParam>, TParser>();
            return collection;
        }

        private static void AddDefaultParameterParsers(this IServiceCollection collection)
        {
            collection.AddTelegramBotParameterParser<long, LongParameter>()
                      .AddTelegramBotParameterParser<int, IntParameter>()
                      .AddTelegramBotParameterParser<string, StringParametr>()
                      .AddTelegramBotParameterParser<bool, BoolParameter>()
                      .AddTelegramBotParameterParser<float, FloatParameter>()
                      .AddTelegramBotParameterParser<double, DoubleParameter>()
                      .AddTelegramBotParameterParser<decimal, DecimalParameter>()
                      .AddTelegramBotParameterParser<DateTime, DateTimeParameter>()
                      .AddTelegramBotParameterParser<DateTimeOffset, DateTimeOffsetParameter>()
                      .AddTelegramBotParameterParser<TimeSpan, TimeSpanParameter>();
        }

        public static void AddTelegramBotInMemoryStorage(this IServiceCollection collection)
        {
            collection.AddSingleton<ISessionProvider, InMemorySessionProvider>();
            collection.AddSingleton<IUserProvider, DefaultUserProvider>();
        }
    }
}
