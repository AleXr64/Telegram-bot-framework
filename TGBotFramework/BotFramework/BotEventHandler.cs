using BotFramework.Abstractions;
using BotFramework.Attributes;
using BotFramework.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework
{
    public abstract class BotEventHandler
    {
        private HandlerParams _params;

        protected Chat Chat => _params.Chat;
        protected User From => _params.From;
        protected Update RawUpdate => _params.Update;
        protected ITelegramBotClient Bot => _params.Bot;
        protected bool HasChat => _params.HasChat;
        protected bool HasFrom => _params.HasFrom;
        protected bool IsCallbackQuery => CallbackQuery != null;
        protected CallbackQuery CallbackQuery => _params.CallbackQuery;

        protected IBotRequestContext Context => _params;

        internal void __Instantiate(IBotRequestContext requestContext) { _params = requestContext as HandlerParams; }
    }

    internal class EventHandler
    {
        public HandlerAttribute Attribute;
        public Type MethodOwner;
        public MethodInfo Method;
        public ParameterInfo[] Parameters;
        public bool Parametrized;
    }

    internal class Method
    {
        public List<EventHandler> Handlers = new();
        public MethodInfo Info;
        public InChat InChat;
        public short Priority;
        public Type Owner;
        public ConditionType ConditionType;
    }

    internal class EventHandlerFactory
    {
        private readonly List<Method> _methods = new();
        
        public void Find()
        {
            var knowHandlers = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                      .Where(x => x.FullName != null)
                                      .GroupBy(x => x.FullName)
                                      .Select(g => g.First())
                                      .ToArray(); //workaround for duplicated assemblies


            foreach(var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                                        .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(BotEventHandler)));
                    knowHandlers.AddRange(types);

                } catch(ReflectionTypeLoadException) { }
            }


            foreach(var handler in knowHandlers)
            {
                var methods = handler.GetMethods().Where(x => 
                    x.GetCustomAttributes<HandlerAttribute>().Any());

                foreach(var methodInfo in methods)
                {
                    var priority = methodInfo.GetCustomAttribute<PriorityAttribute>();
                    var inchat = methodInfo.GetCustomAttribute<InChatAttribute>();
                    if(priority != null && methodInfo.ReturnType != typeof(Task<bool>))
                    {
                        throw new Exception($"Method {methodInfo.Name} should return Task<bool> when priority attribute used");
                    }

                    var conditionType = methodInfo.GetCustomAttribute<HandleConditionAttribute>()?.ConditionType ??
                                        ConditionType.Any;

                    var method = new Method 
                        {
                            Info = methodInfo,
                            Priority = priority?.Value ?? 0,
                            ConditionType = conditionType,
                            Owner = handler,
                            InChat = inchat?.ChatType ?? InChat.All
                        };

                    var attributes = methodInfo.GetCustomAttributes<HandlerAttribute>();
                    foreach(var attribute in attributes)
                    {
                        var eHandler = new EventHandler 
                            {
                                Attribute = attribute,
                                MethodOwner = handler,
                                Method = methodInfo
                            };

                        eHandler.Parametrized = eHandler.Attribute is ParametrizedCommandAttribute;
                        eHandler.Parameters = methodInfo.GetParameters();
                        method.Handlers.Add(eHandler);
                    }
                    _methods.Add(method);
                }
            }
        }

        public async Task ExecuteHandler(HandlerParams param)
        {
            var methods = _methods
                         .Where(m => m.InChat == InChat.All || m.InChat == param.InChat )
                         .OrderByDescending(m => m.Priority);// in case handlers was added in runtime

            var availableHandlers = new List<EventHandler>();
            foreach(var method in methods)
            {
                if(method.ConditionType == ConditionType.Any)
                {
                    var handlers = method.Handlers.Where(handler => handler.Attribute.CanHandleInternal(param)).ToList();
                    if(handlers.Any())
                        availableHandlers.Add(handlers.First());
                }
                else if(method.Handlers.All(handler => handler.Attribute.CanHandleInternal(param)))
                {
                    availableHandlers.Add(method.Handlers.FirstOrDefault());
                }
            }

            foreach(var eventHandler in availableHandlers)
            {
                var executed = await Exec(eventHandler, param);
                switch(executed)
                {
                    case HandlerExec.Break:
                        return;
                    case HandlerExec.Error:
                        //TODO: ?
                        break;
                }
            }

        }

        private async Task<HandlerExec> Exec(EventHandler handler, HandlerParams param)
        {
            var provider = param.ServiceProvider;
            var method = handler.Method;

            try
            {
                var instance = (BotEventHandler)ActivatorUtilities.CreateInstance(provider, handler.MethodOwner);
                instance.__Instantiate(param);
                object[] paramObjects;
                if(handler.Parameters.Length > 0 && handler.Parametrized && param.IsParametrizedCommand)
                {
                    var parseOk = param.TryParseParams(handler.Parameters);

                    var paramses = handler.Parameters;
                    if(parseOk)
                        paramObjects =
                            paramses.Select(x =>
                                         {
                                             return param.CommandParameters
                                                         .First(p => p.Position == x.Position)
                                                         .TypedValue;
                                         })
                                    .ToArray();
                    else
                        return HandlerExec.Error;
                }
                else
                {
                    paramObjects = null;
                }
                var task = method.Invoke(instance, paramObjects);
                if(task == null)
                {
                    return HandlerExec.Continue;
                }

                if(method.ReturnParameter?.ParameterType == typeof(Task))
                {
                    
                    await (Task)task;
                    return HandlerExec.Continue;
                }

                if(method.ReturnParameter?.ParameterType == typeof(Task<bool>))
                {
                    return await (Task<bool>)task ? HandlerExec.Continue : HandlerExec.Break;
                }

            } catch(ArgumentException e)
            {
                Console.WriteLine(e);
            }

            return HandlerExec.Error;
        }
    }
}
