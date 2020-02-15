using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotAsWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTelegramBot();
                    services.AddTelegramBotRawUpdateParser<MeParam, MeParser>();
                });
    }
}
