using System;
using System.Net;
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
    private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

    private readonly ITelegramBotClient _client;
    private readonly IUpdateTarget _updateTarget;
    private readonly BotConfig _config;

    private readonly string? _secretToken;

    private readonly CancellationTokenSource _canRunTokenSource = new ();

    public WebhookUpdateProvider(ITelegramBotClient client, IUpdateTarget updateTarget, IOptions<BotConfig> config)
    {
        _client = client;
        _updateTarget = updateTarget;
        _config = config.Value;

        _secretToken = _config.Webhook.SecretToken;
        if (string.IsNullOrWhiteSpace(_secretToken))
        {
            _secretToken = null;
        }
    }

    public async Task StartAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var server = new WatsonWebserver.Webserver(GetWebserverSettings(), Route);
        _ = server.StartAsync(_canRunTokenSource.Token);

        await _client.SetWebhook(_config.Webhook.Url, secretToken: _secretToken, cancellationToken: token);
    }
    
    public async Task StopAsync(CancellationToken token)
    {
        _canRunTokenSource.Cancel();
        await _client.DeleteWebhook(cancellationToken: token);
    }

    private async Task Route(HttpContextBase ctx)
    {
        var request = ctx.Request;
        if (_secretToken != null && request.RetrieveHeaderValue(SecretTokenHeader) != _secretToken)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            
            await ctx.Response.Send("Missing or invalid secret_token");
            return;
        }

        try
        {
            var obj = JsonConvert.DeserializeObject<Update>(request.DataAsString);
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
