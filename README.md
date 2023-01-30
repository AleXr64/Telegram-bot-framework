# Telegram-bot-framework [![Nuget](https://img.shields.io/nuget/v/AleXr64.BotFramework?logo=nuget)](https://www.nuget.org/packages/AleXr64.BotFramework/) [![Telegram chat](https://img.shields.io/badge/Telegram-TGBotFramework-blue?logo=telegram)](https://t.me/tgbotframework)
Framework to simplify writing telegram bots.
Register framework as service in DI conatiner:

    services.AddTelegramBot();
Inherit from the BotEventHandler class and add new method with specified attributes:

```csharp
    public class EventHandler:BotEventHandler
    {
        // Answer on "/start" command from private messages
        [Command(InChat.Private, "start")]
        public async Task Start() => await Bot.SendTextMessageAsync(Chat, "Hello! U started me =)");

        //Answer on message with "ban" text
        [Message("ban")]
        public async Task Ban() => await Bot.SendTextMessageAsync(Chat, "I will ban you right now! Just kidding");

        //Answer on message that satisfy provided regex expression
        [Message("^.*?(?i)python$", regex: true)]
        public async Task Task() => await Bot.SendTextMessageAsync(Chat, "I hate snakes");

        //Answer on any update
        [Update(InChat.All, UpdateFlag.All)]
        public async Task Update() => await Bot.SendTextMessageAsync(Chat, "Hello");

        //Answer on message that contains photo or video
        [Message(InChat.All, MessageFlag.HasPhoto | MessageFlag.HasVideo)]
        public async Task PhotoVideo() => await Bot.SendTextMessageAsync(Chat, "Send me more!");
    }
```
And enjoy =)

Example project can be found [here](TGBotFramework/Examples)

Requires:

 - .netstandard 2.0
 - [Microsoft Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)  (via asp.net core or hosted service template)

