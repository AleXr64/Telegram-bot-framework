using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotFramework.Storage.EFCore.Internal
{
    internal class StoreUsersService: IHostedService
    {
        private Thread _thread;
        private AutoResetEvent _event = new AutoResetEvent(false);
        private List<UserRecord> _knowRecords = new List<UserRecord>();
        private bool _runing;

        private class UserRecord
        {
            private readonly long _userId;
            private readonly long _chatId;

            public UserRecord(long chatId, long userId)
            {
                _chatId = chatId;
                _userId = userId;
                TimeStamp = DateTime.Now;
            }

            public long UserId {
                get
                {
                    TimeStamp = DateTime.Now;
                    return _userId;
                }
            }

            public long ChatId {
                get
                {
                    TimeStamp = DateTime.Now;
                    return _chatId;
                }
            }

            public DateTime TimeStamp { get; private set; }
        }

        private async Task Cycle(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested || _runing)
            {
                _event.WaitOne();
                using(var scope = scopeFactory.CreateScope())
                {
                    while(queue.TryDequeue(out var ent))
                    {
                        var db = scope.ServiceProvider.GetService(_contexType) as BotStorageContext;
                        var handlers = scope.ServiceProvider.GetServices<ITelegramBotNewUsersHandler>().ToArray();

                        if(ent is TelegramUser user)
                        {
                            var know = _knowRecords.FirstOrDefault(x => x.ChatId == user.Chat.RealId &&
                                                                        x.UserId == user.RealId);
                            if(know == null)
                            {
                                _knowRecords.Add(new UserRecord(user.Chat.RealId, user.RealId));
                              await DbQuery(user.RealId, user.Chat.RealId, db, handlers);
                            }
                        }
                    }
                }

                _knowRecords.RemoveAll(x => (DateTime.Now - x.TimeStamp).TotalMinutes > 30);
            }
        }


        private async Task DbQuery(long userId, long chatId, BotStorageContext db, ITelegramBotNewUsersHandler[] handlers)
        {
            if(!db.Users.AsQueryable()
                                 .Any(x => x.RealId == userId && x.Chat.RealId == chatId))
            {
                var chat = db.Chats.AsQueryable()
                             .FirstOrDefault(x => x.RealId == chatId);
                var user = new TelegramUser();
                user.Id = Guid.NewGuid();
                user.RealId = userId;
                user.Chat = chat ?? new TelegramChat { Id = Guid.NewGuid() };

                db.Add(user);
                db.SaveChanges();

                foreach(var handler in handlers)
                {
                    await handler.SaveAction(user, user.Chat);
                }
            }
        }


        private IServiceScopeFactory scopeFactory;
        private Type _contexType;
        private ConcurrentQueue<TelegramStorageEntity> queue = new ConcurrentQueue<TelegramStorageEntity>();

        public StoreUsersService(IServiceScopeFactory scopeFactory, Type type)
        {
            this.scopeFactory = scopeFactory;
            _contexType = type;
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public async Task AddToQueue(TelegramStorageEntity entity)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            queue.Enqueue(entity);
            _event.Set();
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public async Task StartAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            _runing = true;
            _thread = new Thread(async (t) => await Cycle((CancellationToken)t));
            _thread.Start(cancellationToken);
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public async Task StopAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            _runing = false;
            _event.Set();
        }
    }
}
