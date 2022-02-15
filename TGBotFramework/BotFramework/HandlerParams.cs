using System;
using System.Collections.Generic;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Abstractions.Storage;
using BotFramework.Enums;
using BotFramework.Session;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework
{
    public sealed class HandlerParams : IBotRequestContext
    {
        internal readonly IServiceProvider ServiceProvider;
        private readonly string UserName;

        internal HandlerParams(IBotInstance bot, Update update, IServiceProvider serviceProvider, string userName, IUserProvider userProvider)
        {
            ServiceProvider = serviceProvider;
            Instance = bot;
            Update = update;
            UserName = userName;

            PrepareChat();

            CheckForCommand();

            if(HasFrom)
                BotUser = userProvider.GetUser(From.Id, Chat?.Id ?? From.Id);

            SessionProvider = serviceProvider.GetService<ISessionProvider>() ?? new InMemorySessionProvider();
        }

        private void PrepareChat()
        {
            
            switch(Update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    From = Update.Message.From;
                    Chat = Update.Message.Chat;

                    break;
                case UpdateType.InlineQuery:
                    From = Update.InlineQuery.From;

                    break;
                case UpdateType.ChosenInlineResult:
                    From = Update.ChosenInlineResult.From;

                    break;
                case UpdateType.CallbackQuery:
                    From = Update.CallbackQuery.From;
                    Chat = Update.CallbackQuery.Message.Chat;
                    //     var __chat = Bot.GetChatAsync(update.CallbackQuery.ChatInstance);
                    //   __chat.Wait();
                    //     Chat = __chat.Result;
                    CallbackQuery = Update.CallbackQuery;

                    break;
                case UpdateType.EditedMessage:
                    From = Update.EditedMessage.From;
                    Chat = Update.EditedMessage.Chat;

                    break;
                case UpdateType.ChannelPost:
                    From = Update.ChannelPost.From;
                    Chat = Update.ChannelPost.Chat;

                    break;
                case UpdateType.EditedChannelPost:
                    From = Update.EditedChannelPost.From;
                    Chat = Update.EditedChannelPost.Chat;

                    break;
                case UpdateType.ShippingQuery:
                    From = Update.ShippingQuery.From;

                    break;
                case UpdateType.PreCheckoutQuery:
                    From = Update.PreCheckoutQuery.From;

                    break;
                case UpdateType.Poll:
                case UpdateType.PollAnswer:
                case UpdateType.MyChatMember:
                case UpdateType.ChatMember:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(HasChat)
            {
                InChat = Chat.Type switch
                             {
                                 ChatType.Private => InChat.Private,
                                 ChatType.Group => InChat.Public,
                                 ChatType.Channel => InChat.Channel,
                                 ChatType.Supergroup => InChat.Public,
                                 _ => InChat.All
                             };
            }
        }

        public List<CommandParameter> CommandParameters { get; } = new List<CommandParameter>();
        public string[] CommandArgs { get; private set; } = { string.Empty };

        public string CommandName { get; private set; } = string.Empty;
        public bool IsCommand { get; private set; }
        public bool IsFullFormCommand { get; private set; } = false;
        public Chat Chat { get; set; }

        public User From { get; set; }

        public IBotInstance Instance { get; }

        public Update Update { get; }

        public IReadOnlyList<IBotRequestHandler> PossibleHandlers { get; }

        public IBotUser BotUser { get; private set; }

        public ISessionProvider SessionProvider { get; }

        public ITelegramBotClient Bot => Instance.BotClient;

        public bool HasChat => Chat != null;
        public bool HasFrom => From != null;
        public UpdateType Type => Update.Type;
        public InChat InChat { get; set; }

        public CallbackQuery CallbackQuery { get; set; }

        private void CheckForCommand()
        {
            if(Update.Type != UpdateType.Message)
                return;

            if(Update.Message?.Type != MessageType.Text)
                return;

            IsCommand = Update.Message.Text.IsCommand();
            if(IsCommand)
            {
                IsFullFormCommand = Update.Message.Text.IsCommand(UserName);
                CommandName = Update.Message.Text.GetCommandName(IsFullFormCommand ? UserName : "");
                CommandArgs = Update.Message.Text.GetCommandArgs();
            }
        }

        internal bool TryParseParams(ParameterInfo[] parameters)
        {
            foreach(var parameterInfo in parameters)
            {
                var position = parameterInfo.Position;

                var getServiceMethod = typeof(IServiceProvider).GetMethod("GetService");
                if(getServiceMethod == null) throw new AccessViolationException("How this happens?");

                GetParam(getServiceMethod, position, parameterInfo.ParameterType);
            }

            return parameters.Length == CommandParameters.Count;
        }

        private void MakeUser()
        {

        }

        private void GetParam(MethodInfo getServiceMethod, int position, Type parameterType)
        {
            var baseParserType = typeof(IParameterParser<>);
            var parserType = baseParserType.MakeGenericType(parameterType);
            var parser = getServiceMethod.Invoke(ServiceProvider, new object[] { parserType });



            if(parser != null)
                try
                {
                    var defaultInstance =
                        parser.GetType().GetMethod("DefaultInstance")?.Invoke(parser, null);

                    var parserParams = new[] { CommandArgs[position], defaultInstance };
                    var result = parser.GetType().GetMethod("TryGetValue")?.Invoke(parser, parserParams);
                    if(result != null && (bool)result)
                    {
                        var commandParameter = new CommandParameter(position, parserParams[1]);
                        CommandParameters.Add(commandParameter);
                        return;
                    }
                } catch
                {
                    throw new ArgumentException("Wrong parser! WTF??");
                }

            var extendParametrType = typeof(IRawParameterParser<>);
            parserType = extendParametrType.MakeGenericType(parameterType);
            parser = getServiceMethod.Invoke(ServiceProvider, new object[] { parserType });
            if(parser != null)
                try
                {
                    var defaultInstance =
                        parser.GetType().GetMethod("DefaultInstance")?.Invoke(parser, null);

                    var parserParams = new[] { Update, defaultInstance };
                    var result = parser.GetType().GetMethod("TryGetValueByRawUpdate")?.Invoke(parser, parserParams);
                    if(result != null && (bool)result)
                    {
                        var commandParameter =
                            new CommandParameter(position, parserParams[1]);
                        CommandParameters.Add(commandParameter);
                    }
                } catch
                {
                    throw new ArgumentException("Wrong parser! WTF??");
                }
        }
    }
}
