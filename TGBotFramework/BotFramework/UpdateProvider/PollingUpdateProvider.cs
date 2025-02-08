using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions.UpdateProvider;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotFramework.UpdateProvider;

public class PollingUpdateProvider: IUpdateProvider
{
    private readonly IUpdateTarget _updateTarget;
    private readonly ITelegramBotClient _client;

    private readonly CancellationTokenSource _canRunTokenSource = new();

    public PollingUpdateProvider(ITelegramBotClient client, IUpdateTarget updateTarget)
    {
        _updateTarget = updateTarget;
        _client = client;
    }

    public async Task StartAsync(CancellationToken token)
    {
        await _client.DeleteWebhook(cancellationToken: token);
        token.ThrowIfCancellationRequested();
        _client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions { DropPendingUpdates = false },
            _canRunTokenSource.Token
        );
    }

    private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _updateTarget.Push(update);

        return Task.CompletedTask;
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        await Task.Delay(5000, cancellationToken); //иначе будет долбиться и грузить проц на 100%
    }

    public Task StopAsync(CancellationToken token)
    {
        _canRunTokenSource.Cancel();
        return Task.CompletedTask;
    }
}
