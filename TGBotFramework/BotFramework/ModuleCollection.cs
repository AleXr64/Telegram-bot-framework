using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework
{
    public sealed class ModuleCollection
    {
        public ModuleCollection()
        {
            throw new AccessViolationException("This class can not be instantiated manually!");
        }

        internal ModuleCollection(IEnumerable<IBotFrameworkModule> modules) { Modules = modules.ToList().AsReadOnly(); }
        internal IReadOnlyList<IBotFrameworkModule> Modules { get; }
    }

    public static class ModuleCollectionExtensions
    {
        public static TModule GetModule<TModule>(this ModuleCollection collection) where TModule : class, IBotFrameworkModule
        {
           return collection.Modules.FirstOrDefault(x => x.GetType() == typeof(TModule)) as TModule;
        }
    }
}
