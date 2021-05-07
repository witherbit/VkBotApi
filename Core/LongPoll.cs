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
        private Api _api;

        public event EventHandler<UpdateEventArgs> OnUpdate;
        public event EventHandler<CancellationToken> OnStart;
        public event EventHandler<CancellationToken> OnStop;
        public event EventHandler<CancellationToken> OnDispose;

        public long GroupId { get; private set; }

        internal LongPoll(Api api, long groupId)
        {
            _api = api;
            GroupId = groupId;
        }

        private CancellationTokenSource _cts;
        public CancellationToken CancellToken { get; private set; }

        public string Ts { get; private set; }

        public string Key { get; private set; }

        public string Server { get; private set; }

        private void GetInfoLongPoll()
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
                            UpdateEventArgs update = new UpdateEventArgs(response);
                            Task.Factory.StartNew(() => OnUpdate?.Invoke(this, update));
                        }
                    }
                }
            }
            else Api.SendException(this, new ApiException("The task cannot be started because it is already running", ExceptionCode.System));
        }

        public async void ListenAsync()
        {
            await Task.Run(()=>
            {
                Listen();
            });
        }

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
            GC.Collect();
        }

        public override string ToString()
        {
            return _api.Token + " " + GroupId;
        }

        ~LongPoll()
        {
            Dispose();
        }
    }
}
