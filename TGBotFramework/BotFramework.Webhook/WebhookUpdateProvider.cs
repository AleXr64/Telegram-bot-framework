using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WatsonWebserver.Core;

namespace BotFramework.Webhook;

public class WebhookUpdateProvider : IUpdateProvider
{
    private readonly ITelegramBotClient _client;
    private readonly IUpdateSource _updateSource;
    private readonly BotConfig _config;

    public WebhookUpdateProvider(ITelegramBotClient client, IUpdateSource updateSource, IOptions<BotConfig> config)
    {
        _client = client;
        _updateSource = updateSource;
        _config = config.Value;
    }

    public async Task StartAsync(CancellationToken token)
    {
        var server = new WatsonWebserver.Webserver(GetWebserverSettings(), Route);
        _ = server.StartAsync(token);

        await _client.SetWebhookAsync(_config.Webhook.Url, cancellationToken: token);
    }
    
    public async Task StopAsync(CancellationToken token)
    {
        await _client.DeleteWebhookAsync(cancellationToken: token);
    }

    private async Task Route(HttpContextBase ctx)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject<Update>(ctx.Request.DataAsString);
            if (obj != null)
            {
                _updateSource.Push(obj);
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
