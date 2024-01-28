namespace BotFramework.Config;

public class WebhookConfig
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string Certificate { get; set; }
    public string SecretToken { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}
