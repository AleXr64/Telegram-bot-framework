namespace BotFramework.Config
{
    public class BotConfig
    {
        public string Token { get; set; }
        public WebhookConfig Webhook { get; set; }
        public string SOCKS5Address { get; set; }
        public int SOCKS5Port { get; set; }
        public string SOCKS5User { get; set; }
        public string SOCKS5Password { get; set; }
        public bool UseSOCKS5 { get; set; }
        public bool UseTestEnv { get; set; }
        public string BotApiUrl { get; set; }
    }
}
