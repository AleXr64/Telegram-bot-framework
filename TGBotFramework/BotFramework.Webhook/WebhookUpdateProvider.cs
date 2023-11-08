using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions.UpdateProvider;
using BotFramework.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WatsonWebserver.Core;

namespace BotFramework.Webhook;

public class WebhookUpdateProvider : IWebhookProvider
{
    private readonly ITelegramBotClient _client;
    private readonly IUpdateTarget _updateTarget;
    private readonly BotConfig _config;

    private readonly CancellationTokenSource _canRunTokenSource = new ();

    public WebhookUpdateProvider(ITelegramBotClient client, IUpdateTarget updateTarget, IOptions<BotConfig> config)
    {
        _client = client;
        _updateTarget = updateTarget;
        _config = config.Value;
    }

    public async Task StartAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var server = new WatsonWebserver.Webserver(GetWebserverSettings(), Route);
        _ = server.StartAsync(_canRunTokenSource.Token);

        await _client.SetWebhookAsync(_config.Webhook.Url, cancellationToken: token);
    }
    
    public async Task StopAsync(CancellationToken token)
    {
        _canRunTokenSource.Cancel();
        await _client.DeleteWebhookAsync(cancellationToken: token);
    }

    private async Task Route(HttpContextBase ctx)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject<Update>(ctx.Request.DataAsString);
            if (obj != null)
            {
                _updateTarget.Push(obj);
            }
        } catch(Exception e)
        {
            Console.WriteLine(e);
        }

        await ctx.Response.Send();
    }

    private WebserverSettings GetWebserverSettings()
    {
        return new WebserverSettings(_config.Webhook.Host, _config.Webhook.Port);
    }
}
