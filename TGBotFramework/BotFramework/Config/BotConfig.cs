namespace BotFramework.Config
{
    public class BotConfig
    {
        public string Token { get; set; }
        public string WebHookURL { get; set; }
        public bool UseCertificate { get; set; }
        public string WebHookCertPath { get; set; }
        public bool EnableWebHook { get; set; }
        public string SOCKS5Address { get; set; }
        public int SOCKS5Port { get; set; }
        public string SOCKS5User { get; set; }
        public string SOCKS5Password { get; set; }
        public bool UseSOCKS5 { get; set; }
        public bool UseTestEnv { get; set; }
        public string BotApiUrl { get; set; }
    }
}
