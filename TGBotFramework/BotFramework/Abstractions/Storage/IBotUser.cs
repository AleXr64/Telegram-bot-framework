namespace BotFramework.Abstractions.Storage
{
    public interface IBotUser : IBotEntity
    {
        IBotChat Chat { get; set; }
    }
}
