using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Abstractions.UpdateProvider;
using BotFramework.Config;
using BotFramework.Middleware;
using BotFramework.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class Bot: IHostedService, IBotInstance, IUpdateTarget
    {
        private readonly BotConfig botConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private IUpdateProvider _updateProvider;
        private readonly LinkedList<Type> _wares;
        private EventHandlerFactory _factory;
        private readonly CancellationTokenSource _receiveToken = new();

        public string UserName { get; private set; }
        public ITelegramBotClient BotClient { get; }

        private readonly ConcurrentQueue<Update> _updateQueue = new();
        private readonly ManualResetEvent _shouldProcess = new(false);

        public Bot(IServiceProvider serviceProvider,
                   IOptions<BotConfig> options,
                   IServiceScopeFactory scopeFactory, 
                   ITelegramBotClient client,
                   Type startupType = null
            )
        {
            _serviceProvider = serviceProvider;
            _scopeFactory = scopeFactory;
            BotClient = client;
            botConfig = options.Value;


            if(startupType != null)
            {
                _wares = ((BotStartup)Activator.CreateInstance(startupType, true)).__SetupInternal();
            }
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            if(botConfig.Webhook.Enabled)
            {
                _updateProvider = _serviceProvider.GetService<IWebhookProvider>();
            }

            _updateProvider ??= _serviceProvider.GetService<IUpdateProvider>();
            // TODO: do smth if there are no registered provider?

            await _updateProvider.StartAsync(cancellationToken);
            await StartListen(cancellationToken);
        }

        async Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            await _updateProvider.StopAsync(cancellationToken);
            _receiveToken.Cancel();
        }


        private async Task StartListen(CancellationToken cancellationToken)
        {
            try
            {
                _factory = new EventHandlerFactory();
                _factory.Find();

                var me = await GetMeSafeAsync(cancellationToken);
                UserName = me.Username;
                Console.WriteLine();
                Console.WriteLine($"    Telegram Bot Framework {ThisAssembly.AssemblyVersion}");
                Console.WriteLine();
                Console.WriteLine($"        {UserName} started!");
                Console.WriteLine();
            } catch(Exception e) when(e is ArgumentException)
            {
                Console.WriteLine(e);

                throw;
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }

            var updateThread = new Thread(UpdateThread) { Name = "Update handler thread" };
            updateThread.Start(_receiveToken.Token);
        }

        private void UpdateThread(object token)
        {
            var cancellationToken = (CancellationToken) token;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var wasSignaled = _shouldProcess.WaitOne(TimeSpan.FromSeconds(1));
                if (wasSignaled)
                {
                    if (_updateQueue.TryDequeue(out var update))
                    {
                        _ = HandleUpdateAsync(update, cancellationToken);
                    }

                    _shouldProcess.Reset();
                }
            }
        }

        private async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
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

                        var router = new Router(_factory);
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
        
        private async Task<User> GetMeSafeAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await BotClient.GetMeAsync(cancellationToken);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(5000, cancellationToken);

            return await GetMeSafeAsync(cancellationToken);
        }

        public void Push(Update update)
        {
            _updateQueue.Enqueue(update);
            _shouldProcess.Set();
        }
    }
}
