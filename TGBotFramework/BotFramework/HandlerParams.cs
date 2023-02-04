using BotFramework.Setup;
using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework
{
    public sealed class HandlerParams
    {
        internal readonly IServiceProvider ServiceProvider;
        private readonly string UserName;

        internal HandlerParams(ITelegramBotClient bot, Update update, IServiceProvider serviceProvider, string userName)
        {
            ServiceProvider = serviceProvider;
            Bot = bot;
            Update = update;
            UserName = userName;
            switch(update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    From = update.Message.From;
                    Chat = update.Message.Chat;
                    break;
                case UpdateType.InlineQuery:
                    From = update.InlineQuery.From;
                    break;
                case UpdateType.ChosenInlineResult:
                    From = update.ChosenInlineResult.From;
                    break;
                case UpdateType.CallbackQuery:
                    From = update.CallbackQuery.From;
                    //     var __chat = Bot.GetChatAsync(update.CallbackQuery.ChatInstance);
                    //   __chat.Wait();
                    //     Chat = __chat.Result;
                    CallbackQuery = update.CallbackQuery;
                    break;
                case UpdateType.EditedMessage:
                    From = update.EditedMessage.From;
                    Chat = update.EditedMessage.Chat;
                    break;
                case UpdateType.ChannelPost:
                    From = update.ChannelPost.From;
                    Chat = update.ChannelPost.Chat;
                    break;
                case UpdateType.EditedChannelPost:
                    From = update.EditedChannelPost.From;
                    Chat = update.EditedChannelPost.Chat;
                    break;
                case UpdateType.ShippingQuery:
                    From = update.ShippingQuery.From;
                    break;
                case UpdateType.PreCheckoutQuery:
                    From = update.PreCheckoutQuery.From;
                    break;
                case UpdateType.Poll:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(HasChat)
                switch(Chat.Type)
                {
                    case ChatType.Private:
                        InChat = InChat.Private;
                        break;
                    case ChatType.Group:
                        InChat = InChat.Public;
                        break;
                    case ChatType.Channel:
                        InChat = InChat.Channel;
                        break;
                    case ChatType.Supergroup:
                        InChat = InChat.Public;
                        break;
                    default:
                        InChat = InChat.All;
                        break;
                }

            CheckForCommand();
        }

        public List<CommandParameter> CommandParameters { get; } = new List<CommandParameter>();
        public string[] CommandArgs { get; private set; } = { string.Empty };

        public string CommandName { get; private set; } = string.Empty;
        public bool IsCommand { get; private set; }
        public bool IsFullFormCommand { get; private set; } = false;
        public Chat Chat { get; }
        public User From { get; }
        public Update Update { get; }
        public ITelegramBotClient Bot { get; }
        public bool HasChat => Chat != null;
        public bool HasFrom => From != null;
        public UpdateType Type => Update.Type;
        public InChat InChat { get; }
        public CallbackQuery CallbackQuery { get; }

        private void CheckForCommand()
        {
            if(Update.Type != UpdateType.Message)
                return;

            if(Update.Message.Type != MessageType.Text)
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
