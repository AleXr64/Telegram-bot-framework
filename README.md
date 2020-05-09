# Telegram-bot-framework ![Nuget](https://img.shields.io/nuget/v/AleXr64.BotFramework)
Framework to simplify writing telegram bots.
Register framework as service in DI conatiner:

    services.AddTelegramBot();
Inherit from the BotEventHandler class:

    public class EventHandler:BotEventHandler
    {
    ...
Add new method, mark that with Attribute:

    [Command("start")]
    public async Task Start() => await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");
    }

And enjoy =)

Example project can be found [here](https://github.com/AleXr64/Telegram-bot-framework-examples)

Requires:

 - .net core 3.1
 - [microsoft dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)  (via asp.net core or hosted service template)

