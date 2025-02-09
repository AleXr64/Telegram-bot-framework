using System.Collections.Generic;
using System.Linq;
using BotFramework;
using BotFramework.Attributes;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BotFramework.Abstractions.Storage;
using BotFramework.Enums;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MsgDumpBot
{
    class EventHandler:BotEventHandler
    {
        [Command("msgdump", CommandParseMode.Both)]
        public async Task MsgDump()
        {
            var msg = RawUpdate.Message;
            if(msg.ReplyToMessage != null)
            {
                string jmsg = JsonConvert.SerializeObject(msg, Formatting.Indented);
                await Bot.SendMessage(Chat.Id, $"<code>{jmsg}</code>", parseMode: ParseMode.Html);
            }
        }

        [ParametrizedCommand("store", CommandParseMode.Both)]
        public async Task StoreSession(string arg)
        {
            var keyValues = new LinkedList<KeyValuePair<string, string>>();

            var pairs = arg.Split(":");

            for(int i = 0; i < pairs.Length; i++)
            {
                if(i == 0 || i % 2 == 0)
                {
                    keyValues.AddLast(new KeyValuePair<string, string>(pairs[i], null));
                }
                else
                {
                    var last = keyValues.Last.Value;
                    keyValues.RemoveLast();
                    keyValues.AddLast(new KeyValuePair<string, string>(last.Key, pairs[i]));
                }
            }

            var session = Context.SessionProvider.GetOrCreateSession(Context.BotUser);

            foreach(var pair in keyValues)
            {
                session.AddOrUpdateData(new SessionData(){Type = pair.Key, Data = new MyClass() { Data = pair.Value}});
            }

            Context.SessionProvider.SaveSession(session);

            await Context.Instance.BotClient.SendMessage(Chat, "Stored!");
        }

        [ParametrizedCommand("get", CommandParseMode.Both)]
        public async Task Get(string arg)
        {
            var session = Context.SessionProvider.GetOrCreateSession(Context.BotUser);

            if(session.SessionData.ContainsKey(arg))
            {
                await Bot.SendMessage(Chat, $"Data is: {(session.SessionData[arg] as SessionData).Data.Data}");
            }
            else
            {
                await Bot.SendMessage(Chat, "No data!");
            }

        }
        class MyClass
        {
            public string Data { get; set; }
        }

        class SessionData: ISessionData<MyClass>
        {
            public string Type { get; set; }

            public MyClass Data { get; set; }
        }
    }
}
