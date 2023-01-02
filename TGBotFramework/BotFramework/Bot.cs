using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Config;
using BotFramework.Middleware;
using BotFramework.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class Bot: IHostedService, IBotInstance
    {
        private readonly BotConfig _config = new BotConfig();

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly LinkedList<Type> _wares;

        private readonly IServiceProvider services;

        private TelegramBotClient client;

        private EventHandlerFactory factory;

        public Bot(IServiceProvider services, IServiceScopeFactory scopeFactory, Type startupType = null)
        {
            this.services = services;
            _scopeFactory = scopeFactory;

            if(startupType != null)
            {
                _wares = ((BotStartup)Activator.CreateInstance(startupType, true)).__SetupInternal();
            }
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken) { await StartListen(cancellationToken); }

        async Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            await client.DeleteWebhookAsync(cancellationToken: cancellationToken);
            await client.CloseAsync(cancellationToken);
        }

        public string UserName { get; private set; }

        public ITelegramBotClient BotClient => client;

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
                    var handler = new HttpClientHandler { Proxy = proxy };
                    var httpClient = new HttpClient(handler, true);

                    client = new TelegramBotClient(_config.Token, httpClient);
                }
                else
                {
                    client = new TelegramBotClient(_config.Token);
                }

                factory = new EventHandlerFactory();
                factory.Find();

                var me = await GetMeSafeAsync(cancellationToken);
                UserName = me.Username;

                Console.WriteLine($"{Environment.NewLine}    {UserName} started!{Environment.NewLine}");
            } catch(Exception e) when(e is ArgumentException)
            {
                Console.WriteLine(e);

                throw;
            } catch(Exception e)
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
                // v16.x
                // client.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cancellationToken);

                // v17.x
                await client.DeleteWebhookAsync(false, cancellationToken);

                var receiverOptions = new ReceiverOptions { AllowedUpdates = { }, ThrowPendingUpdates = false };
                client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            }
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(async () =>
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var wares = new Stack<IMiddleware>();

                        var wareInstances = scope.ServiceProvider.GetServices<IMiddleware>()
                                                 .ToDictionary(x => x.GetType());

                        var userProvider = scope.ServiceProvider.GetService<IUserProvider>() ??
                                           new DefaultUserProvider();

                        var param = new HandlerParams(this, update, scope.ServiceProvider, UserName, userProvider);

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

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            await Task.Delay(5000, cancellationToken); //иначе будет долбиться и грузить проц на 100%
        }

        private async Task<User> GetMeSafeAsync(CancellationToken cancellationToken)
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
