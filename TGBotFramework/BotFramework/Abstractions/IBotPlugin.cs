namespace BotFramework.Abstractions
{
    public interface IBotPlugin
    {
        IBotPluginContext<IBotPlugin> GetContext();

        bool IsEnabled { get; }

        void Enable();

        void Disable();
    }
}
