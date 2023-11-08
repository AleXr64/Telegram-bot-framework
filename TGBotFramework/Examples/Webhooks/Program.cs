// See https://aka.ms/new-console-template for more information

using BotFramework.Webhook;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
         {
             services.AddTelegramBotWebhook();
         })
    .Build()
    .Run();