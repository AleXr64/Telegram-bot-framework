namespace BotFramework.Storage.EFCore
{
    public static class ModulesExtensions
    {
        public static Storage<TContext> GetStorage<TContext>(this ModuleCollection collection) where TContext:BotStorageContext
            => collection.GetModule<Storage<TContext>>(); 
    }
}
