using BotFramework;
using Microsoft.Extensions.Hosting;



namespace InteractiveTests
{
    class Program
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
                     });
    }
}