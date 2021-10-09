using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using VkBotApi.Models;
using VkBotApi.Enums;
using System.Threading.Tasks;
using System.Threading;

namespace VkBotApi.Core
{
    public sealed class LongPoll : IDisposable
    {
        
        internal Api _api;
        /// <summary>
        /// LongPoll update events.
        /// </summary>
        public MessageEvents MessageEvents { get; private set; } = new MessageEvents();
        /// <summary>
        /// LongPoll update event.
        /// </summary>
        public event EventHandler<UpdateRaw> OnUpdate;
        /// <summary>
        /// Start LongPoll listening event.
        /// </summary>
        public event EventHandler<CancellationToken> OnStart;
        /// <summary>
        /// Stop LongPoll listening event.
        /// </summary>
        public event EventHandler<CancellationToken> OnStop;
        /// <summary>
        /// Dispose LongPoll instance event.
        /// </summary>
        public event EventHandler<CancellationToken> OnDispose;
        /// <summary>
        /// Community id.
        /// </summary>
        public long GroupId { get; internal set; }
        /// <summary>
        /// Access token.
        /// </summary>
        public string Token { get => _api.Token; }

        private CancellationTokenSource _cts;
        /// <summary>
        /// LongPoll listening cancellation token.
        /// </summary>
        public CancellationToken CancellToken { get; private set; }
        /// <summary>
        /// The number of the last event to receive data from.
        /// </summary>
        public string Ts { get; private set; }
        /// <summary>
        /// Secret session key.
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// LongPoll server address.
        /// </summary>
        public string Server { get; private set; }

        internal void GetInfoLongPoll()
        {
            JToken jtoken = _api.CallMethod("groups.getLongPollServer", new Dictionary<string, object>(1)
            {
                {
                    "lp_version",
                    "3"
                }
            });
            Ts = jtoken["ts"].ToString();
            Key = jtoken["key"].ToString();
            Server = jtoken["server"].ToString();
        }

        private JToken RequestLongPoll()
        {
            NameValueCollection parametrs = new NameValueCollection
            {
                {
                    "act",
                    "a_check"
                },
                {
                    "key",
                    Key
                },
                {
                    "ts",
                    Ts
                },
                {
                    "wait",
                    "25"
                },
                {
                    "mode",
                    "2"
                },
                {
                    "version",
                    "3"
                }
            };
            return JToken.Parse(_api.Request(string.Format("{0}", Server), parametrs));
        }
        /// <summary>
        /// Start LongPoll listening.
        /// </summary>
        public void Listen()
        {
            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
                CancellToken = _cts.Token;
                Task.Factory.StartNew(() => OnStart?.Invoke(this, CancellToken));
                while (!CancellToken.IsCancellationRequested)
                {
                    JToken jtoken = RequestLongPoll();
                    if (Convert.ToInt32(jtoken["failed"]) == 1)
                    {
                        Ts = jtoken["ts"].ToString();
                    }
                    else if (Convert.ToInt32(jtoken["failed"]) == 2 || Convert.ToInt32(jtoken["failed"]) == 3)
                    {
                        GetInfoLongPoll();
                    }
                    else
                    {
                        Ts = jtoken["ts"].ToString();
                        foreach (JToken response in ((IEnumerable<JToken>)jtoken["updates"]))
                        {
                            UpdateHandler(response);
                        }
                    }
                }
            }
            else Api.SendException(this, new ApiException("The task cannot be started because it is already running", ExceptionCode.System));
        }
        /// <summary>
        /// Start LongPoll listening async.
        /// </summary>
        public async void ListenAsync()
        {
            await Task.Run(()=>
            {
                Listen();
            });
        }
        /// <summary>
        /// Stop LongPoll listening.
        /// </summary>
        public void StopListen()
        {
            if(_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
                Task.Factory.StartNew(()=>OnStop?.Invoke(this, CancellToken));
            }
            else Api.SendException(this, new ApiException("The task can't be stopped because it hasn't started yet", ExceptionCode.System));
        }
        public void Dispose()
        {
            StopListen();
            Task.Factory.StartNew(() => OnDispose?.Invoke(this, CancellToken));
            GC.SuppressFinalize(this);
        }

        private void UpdateHandler(JToken update)
        {
            OnUpdate?.Invoke(this, new UpdateRaw(update));
            if (update["type"].ToObject<string>() == "message_new")
                MessageEvents.CallEvent(new Message(update), this);
            else if (update["type"].ToObject<string>() == "message_reply")
                MessageEvents.CallEvent(new MessageReply(update), this);
            else if (update["type"].ToObject<string>() == "message_edit")
                MessageEvents.CallEvent(new MessageEdit(update), this);
            else if (update["type"].ToObject<string>() == "message_allow")
                MessageEvents.CallEvent(new MessageAllow(update), this);
            else if (update["type"].ToObject<string>() == "message_deny")
                MessageEvents.CallEvent(new MessageDeny(update), this);
            else if (update["type"].ToObject<string>() == "message_typing_state")
                MessageEvents.CallEvent(new MessageTypingState(update), this);
            else if (update["type"].ToObject<string>() == "message_event")
                MessageEvents.CallEvent(new MessageEvent(update), this);
        }
        ~LongPoll()
        {
            Dispose();
        }
    }
}
