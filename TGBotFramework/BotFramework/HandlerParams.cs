﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed partial class HandlerParams : IBotRequestContext
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

        public List<CommandParameter> CommandParameters { get; } = new List<CommandParameter>();

        public List<Command> Commands { get; private set; } =  new List<Command>();
        public ParametrizedCommand ParametrizedCmd { get; private set; }
        public bool IsParametrizedCommand { get; private set; }
        public bool HasCommands { get; private set; }
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
        
        public bool HasMessage => Update.Message != null;
        
        public UpdateType Type => Update.Type;
        public InChat InChat { get; set; }

        public CallbackQuery CallbackQuery { get; set; }
        
        private void PrepareChat()
        {
            if(HasMessage)
            {
                From = Update.Message?.From;
                Chat = Update.Message?.Chat;
            }
            
            switch(Update.Type)
            {
                case UpdateType.Unknown:
                    break;
                
                case UpdateType.InlineQuery:
                    From = Update.InlineQuery?.From;
                    break;
                
                case UpdateType.ChosenInlineResult:
                    From = Update.ChosenInlineResult?.From;

                    break;
                case UpdateType.CallbackQuery:
                    From = Update.CallbackQuery?.From;
                    Chat = Update.CallbackQuery?.Message?.Chat;
                    CallbackQuery = Update.CallbackQuery;

                    break;
                case UpdateType.EditedMessage:
                    From = Update.EditedMessage?.From;
                    Chat = Update.EditedMessage?.Chat;

                    break;
                case UpdateType.ChannelPost:
                    From = Update.ChannelPost?.From;
                    Chat = Update.ChannelPost?.Chat;

                    break;
                case UpdateType.EditedChannelPost:
                    From = Update.EditedChannelPost?.From;
                    Chat = Update.EditedChannelPost?.Chat;

                    break;
                case UpdateType.ShippingQuery:
                    From = Update.ShippingQuery?.From;
                    break;
                
                case UpdateType.PreCheckoutQuery:
                    From = Update.PreCheckoutQuery?.From;
                    break;
                
                case UpdateType.Poll:
                    From = Update.Message?.From;
                    Chat = Update.Message?.Chat;
                    break;
                
                case UpdateType.Message:
                case UpdateType.PollAnswer:
                case UpdateType.MyChatMember:
                case UpdateType.ChatMember:
                case UpdateType.ChatJoinRequest:
                case UpdateType.MessageReaction:
                case UpdateType.MessageReactionCount:
                case UpdateType.ChatBoost:
                case UpdateType.RemovedChatBoost:
                case UpdateType.BusinessConnection:
                case UpdateType.BusinessMessage:
                case UpdateType.EditedBusinessMessage:
                case UpdateType.DeletedBusinessMessages:
                case UpdateType.PurchasedPaidMedia:
                default:
                    break;
            }

            if(HasChat)
            {
                InChat = Chat?.Type switch
                             {
                                 ChatType.Private => InChat.Private,
                                 ChatType.Group => InChat.Public,
                                 ChatType.Channel => InChat.Channel,
                                 ChatType.Supergroup => InChat.Public,
                                 _ => InChat.All
                             };
            }
        }

        private void CheckForCommand()
        {
            var message = Update.Message;
            if(message == null || Update.Type != UpdateType.Message || message.Caption == null && message.Type != MessageType.Text)
                return;

            var ents = message.Entities ?? message.CaptionEntities;
            var entValues = (message.EntityValues ?? message.CaptionEntityValues)?.ToList();

            if(ents == null || entValues == null) 
                return;

            var entDictionary = new Dictionary<MessageEntity, string>();
            var index = 0;
            foreach(var ent in ents)
            {
                var value = entValues.ElementAt(index);
                if(ent.Type == MessageEntityType.BotCommand 
                   && CommandHelper.IsMyCommand(value, UserName))
                {
                    entDictionary.Add(ent, value);
                }

                index++;
            }

            if(!entDictionary.Any())
                return;

            if(entDictionary.Count == 1 && entDictionary.First().Key.Offset == 0)
            {
                IsParametrizedCommand = true;
                var cmd = entDictionary.First();
                var fulltext = cmd.Value;

                ParametrizedCmd = new ParametrizedCommand
                    {
                        FullText = fulltext,
                        IsFullCommand = fulltext.Contains('@'),
                        Length = cmd.Key.Length,
                        Offset = cmd.Key.Offset,
                        Name = CommandHelper.GetShortName(fulltext),
                        Args = CommandHelper.GetCommandArgs(message.Text ?? message.Caption)
                    };
            }

            HasCommands = true;
            foreach(var ent in entDictionary)
            {
                var fulltext = ent.Value;
                Commands.Add(new Command()
                    {
                        FullText = fulltext,
                        IsFullCommand = fulltext.Contains('@'),
                        Length = ent.Key.Length,
                        Name = CommandHelper.GetShortName(fulltext),
                        Offset = ent.Key.Offset
                    });
            }
        }

        internal bool TryParseParams(ParameterInfo[] parameters)
        {
            if(!IsParametrizedCommand)
                return true;
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
            ParseParam(parser, position, false);

            var extendParametrType = typeof(IRawParameterParser<>);
            parserType = extendParametrType.MakeGenericType(parameterType);
            parser = getServiceMethod.Invoke(ServiceProvider, new object[] { parserType });
            ParseParam(parser, position, true);
        }


        private bool ParseParam(object parser, int position, bool isRaw)
        {
            if(parser == null)
                return false;
            try
            {
                var defaultInstance =
                    parser.GetType().GetMethod("DefaultInstance")?.Invoke(parser, null);

                object[] parserParams;
                object result;
                if(isRaw)
                {
                    parserParams = new[] { Update, defaultInstance };
                    result = parser.GetType().GetMethod("TryGetValueByRawUpdate")?.Invoke(parser, parserParams);
                }
                else
                {
                    parserParams = new[] { ParametrizedCmd.Args[position], defaultInstance };
                    result = parser.GetType().GetMethod("TryGetValue")?.Invoke(parser, parserParams);
                }

                if(result is true)
                {
                    var commandParameter = new CommandParameter(position, parserParams[1]);
                    CommandParameters.Add(commandParameter);
                    return true;
                }
            } catch
            {
                throw new ArgumentException("Wrong parser! WTF??");
            }
            return false;
        }

    }


}
