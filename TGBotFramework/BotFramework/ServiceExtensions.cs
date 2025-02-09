using System;
using System.Linq;
using System.Net.Http;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Abstractions.UpdateProvider;
using BotFramework.Config;
using BotFramework.Middleware;
using BotFramework.ParameterResolvers;
using BotFramework.Session;
using BotFramework.Setup;
using BotFramework.UpdateProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace BotFramework
{
    public static class ServiceExtensions
    {
        public static void AddTelegramBot(this IServiceCollection collection)
        {
            collection.AddSingleton<IUpdateProvider, PollingUpdateProvider>();

            collection.AddOptions<BotConfig>()
                      .Configure<IConfiguration>((config, configuration) => configuration.GetSection("BotConfig").Bind(config));

            collection.AddDefaultParameterParsers();

            collection.AddHttpClient<ITelegramBotClient>()
                      .AddTypedClient<ITelegramBotClient>((client, provider) =>
                           {
                               var botConfig = provider.GetService<IOptions<BotConfig>>().Value;
                               
                               var apiUrl = botConfig.BotApiUrl;
                               if (string.IsNullOrWhiteSpace(apiUrl))
                               {
                                   apiUrl = null;
                               }

                               var options =
                                   new TelegramBotClientOptions(botConfig.Token, apiUrl, botConfig.UseTestEnv);

                               return new TelegramBotClient(options, client);
                           })
                      .ConfigurePrimaryHttpMessageHandler(provider =>
                           {
                               var botConfig = provider.GetService<IOptions<BotConfig>>().Value;

                               if (!botConfig.UseSOCKS5)
                               {
                                   return new HttpClientHandler();
                               }

                               var proxy = new HttpToSocks5Proxy(botConfig.SOCKS5Address, 
                                                                 botConfig.SOCKS5Port, 
                                                                 botConfig.SOCKS5User,
                                                                 botConfig.SOCKS5Password);
                               
                               return new HttpClientHandler { Proxy = proxy };
                           });

            collection.AddSingleton<IBotInstance, Bot>();
            collection.AddTransient<IHostedService>(x => (Bot)x.GetService<IBotInstance>());
            collection.AddSingleton<IUpdateTarget>(x => (Bot)x.GetService<IBotInstance>());
            
            //won't break existent 0.5 users
            collection.AddTelegramBotInMemoryStorage();
        }

        public static void AddTelegramBot<T>(this IServiceCollection collection) where T: BotStartup, new()
        {
            var startup = new T();
            
            foreach (var ware in startup.__SetupInternal())
            {
                collection.AddScoped(typeof(IMiddleware),ware);
            }

            collection.AddTelegramBot();
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
