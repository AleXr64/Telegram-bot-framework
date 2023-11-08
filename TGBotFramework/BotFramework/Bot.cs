using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Middleware;
using BotFramework.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class Bot: IHostedService, IBotInstance
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUpdateSource _updateSource;
        private readonly IUpdateProvider _updateProvider;
        private readonly LinkedList<Type> _wares;
        private EventHandlerFactory _factory;
        private readonly CancellationTokenSource _receiveToken = new();

        public string UserName { get; private set; }
        public ITelegramBotClient BotClient { get; }

        public Bot(IServiceScopeFactory scopeFactory, 
                   IUpdateSource updateSource, 
                   IUpdateProvider updateProvider,
                   ITelegramBotClient client,
                   Type startupType = null
            )
        {
            _scopeFactory = scopeFactory;
            _updateSource = updateSource;
            _updateProvider = updateProvider;
            BotClient = client;

            if(startupType != null)
            {
                _wares = ((BotStartup)Activator.CreateInstance(startupType, true)).__SetupInternal();
            }
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
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

                var wasSignaled = _updateSource.ShouldProcess.WaitOne(TimeSpan.FromSeconds(1));
                if (wasSignaled)
                {
                    var update = _updateSource.Pull();
                    if (update != null)
                    {
                        _ = HandleUpdateAsync(update, cancellationToken);
                    }

                    _updateSource.ShouldProcess.Reset();
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
    }
}
