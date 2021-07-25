using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Config;
using BotFramework.Middleware;
using BotFramework.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MihaZupan;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace BotFramework
{
    public interface ITelegramBot
    {
        string UserName { get; }
        ITelegramBotClient BotClient { get; }
    }
    public class Bot: IHostedService, ITelegramBot
    {
        private readonly BotConfig _config = new BotConfig();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider services;
        private TelegramBotClient client;
        private EventHandlerFactory factory;

        private readonly LinkedList<Type> _wares;

        private string _userName;

        public Bot(IServiceProvider services, IServiceScopeFactory scopeFactory, Type startupType = null)
        {
            this.services = services;
            _scopeFactory = scopeFactory;

            if(startupType != null)
            {
                _wares = ((BotStartup)Activator.CreateInstance(startupType, true)).__SetupInternal();
            }
        }

        public string UserName => _userName;
        public ITelegramBotClient BotClient => client;

        async Task IHostedService.StartAsync(CancellationToken cancellationToken) { await StartListen(cancellationToken); }

        async Task IHostedService.StopAsync(CancellationToken cancellationToken) 
        {
            await client.DeleteWebhookAsync(cancellationToken:cancellationToken);
            await client.CloseAsync(cancellationToken);
        }

        private async Task StartListen(CancellationToken cancellationToken)
        {
            var configProvider = services.GetService<IConfiguration>();
            var section = configProvider.GetSection("BotConfig");
            section.Bind(_config);
            try
            {
                if(_config.UseSOCKS5)
                {
                    var proxy = new HttpToSocks5Proxy(_config.SOCKS5Address, _config.SOCKS5Port, _config.SOCKS5User,
                                                      _config.SOCKS5Password);
                    client = new TelegramBotClient(_config.Token, proxy);
                }
                else
                {
                    client = new TelegramBotClient(_config.Token);
                }

                factory = new EventHandlerFactory();
                factory.Find();

                var me = await GetMeSafeAsync(cancellationToken);
                _userName = me.Username;
            }
            catch(Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                Console.WriteLine(e);
                throw;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            

            if(_config.EnableWebHook)
            {
                if(_config.UseCertificate)
                {
                    //TODO серт
                    // await client.SetWebhookAsync(_config.WebHookURL, new InputFileStream(new FileStream(_config.WebHookCertPath)))
                }
                else
                {
                    //TODO подготовить и заспавнить контроллер
                    await client.SetWebhookAsync(_config.WebHookURL, cancellationToken: cancellationToken);
                }
            }
            else
            {
                client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cancellationToken);
                
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(async () =>
                {

                    using var scope = _scopeFactory.CreateScope();
                    var wares = new Stack<IMiddleware>();

                    var wareInstances = scope.ServiceProvider.GetServices<IMiddleware>().ToDictionary(x => x.GetType());

                    var param = new HandlerParams(botClient, update, scope.ServiceProvider, _userName);

                    var router = new Router(factory);
                    router.__Setup(null, param);

                    wares.Push(router);

                    if(wareInstances.Count > 0 && _wares.Count > 0)
                    {
                        var firstWareType = _wares.First;

                        if(firstWareType?.Value != null && wareInstances.ContainsKey(firstWareType.Value))
                        {
                            var currentWare = wareInstances[firstWareType.Value];
                            ((BaseMiddleware)currentWare).__Setup(router, param);
                            wares.Push(currentWare);

                            var nextWareType = firstWareType.Next;

                            while(nextWareType?.Next != null)
                            {
                                var prevWare = wares.Pop();
                                currentWare = wareInstances[nextWareType.Value];
                                ((BaseMiddleware)currentWare).__Setup(prevWare, param);
                                wares.Push(currentWare);
                            }
                        }
                    }

                    var runWare = wares.Pop();
                    await ((BaseMiddleware)runWare).__ProcessInternal();

                }, cancellationToken);
            } catch(Exception exception)
            {
                Console.WriteLine(exception);
                // throw;
            }
        }

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            await Task.Delay(5000, cancellationToken);//иначе будет долбиться и грузить проц на 100%
        }

        async Task<User> GetMeSafeAsync(CancellationToken cancellationToken)
        {
            try
            {
                var user = await BotClient.GetMeAsync(cancellationToken);
                return user;
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
            await Task.Delay(5000, cancellationToken);
            return await GetMeSafeAsync(cancellationToken);
        }

    }
}
