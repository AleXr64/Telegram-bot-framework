using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;

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
        private string _userName;

        public Bot(IServiceProvider services, IServiceScopeFactory scopeFactory)
        {
            this.services = services;
            _scopeFactory = scopeFactory;
        }

        public string UserName => _userName;
        public ITelegramBotClient BotClient => client;

        public async Task StartAsync(CancellationToken cancellationToken) { await StartListen(); }

        public async Task StopAsync(CancellationToken cancellationToken) { await Task.Run(client.StopReceiving, cancellationToken); }

        public async Task StartListen()
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
                client.OnUpdate += Client_OnUpdate;
                client.OnReceiveError += Client_OnReceiveError;
                client.OnReceiveGeneralError += Client_OnReceiveGeneralError;
                var me = await client.GetMeAsync();
                _userName = me.Username;
            } catch(ArgumentException e)
            {
                Console.WriteLine(e);
            }

            if(_config.EnableWebHook)
            {
                if(_config.UseSertificate)
                {
                    //TODO серт
                    // await client.SetWebhookAsync(_config.WebHookURL, new InputFileStream(new FileStream(_config.WebHookSertPath)))
                }
                else
                {
                    //TODO подготовить и заспавнить контроллер
                    await client.SetWebhookAsync(_config.WebHookURL);
                }
            }
            else
            {
                client.StartReceiving();
            }
        }

        private async void Client_OnUpdate(object sender, UpdateEventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();

                    var param = new HandlerParams(BotClient, e.Update, scope.ServiceProvider, _userName);
                    await factory.ExecuteHandler(param);
                });
            } catch(Exception exception)
            {
                Console.WriteLine(exception);
               // throw;
            }

        }

        private void Client_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void Client_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
           // throw new NotImplementedException();
        }
    }
}
