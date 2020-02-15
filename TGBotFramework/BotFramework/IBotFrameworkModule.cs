using System.Threading.Tasks;

namespace BotFramework
{
    public interface IBotFrameworkModule
    {
        Task PreHandler(HandlerParams handlerParams);
    }
}
