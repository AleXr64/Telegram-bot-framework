using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotFramework.UpdateProvider;

public class PollingUpdateProvider: IUpdateProvider
{
    private readonly IUpdateSource _updateSource;
    private readonly ITelegramBotClient _client;

    public PollingUpdateProvider(ITelegramBotClient client, IUpdateSource updateSource)
    {
        _updateSource = updateSource;
        _client = client;
    }

    public async Task StartAsync(CancellationToken token)
    {
        await _client.DeleteWebhookAsync(cancellationToken: token);

        _client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions { ThrowPendingUpdates = false },
            token
        );
    }

    private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _updateSource.Push(update);

        return Task.CompletedTask;
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        await Task.Delay(5000, cancellationToken); //иначе будет долбиться и грузить проц на 100%
    }

    public Task StopAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
