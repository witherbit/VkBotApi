using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using VkBotApi.Models;
using VkBotApi.Enums;
using System.Threading.Tasks;

namespace VkBotApi.Core
{
    public sealed class Api
    {
        public delegate void ApiEventHandler(UpdateEventArgs e);
        public event ApiEventHandler OnUpdate;
        private string _token { get; set; }
        private long _groupId { get; set; }

        private bool _isRunning { get; set; }

        public string _ts { get; set; }

        public string _key { get; set; }

        public string _server { get; set; }

        public Api(string token, long groupId)
        {
            _token = token;
            _groupId = groupId;
            GetInfoLongPoll();
        }

        internal string Request(string address)
        {
            string result;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                result = webClient.DownloadString(address);
            }
            return result;
        }

        internal string Request(string address, NameValueCollection parametrs)
        {
            string @string;
            using (WebClient webClient = new WebClient())
            {
                @string = Encoding.UTF8.GetString(webClient.UploadValues(address, parametrs));
            }
            return @string;
        }

        public JToken CallMethod(string methodName, Dictionary<string, object> parameters)
        {
            string address = string.Format("https://api.vk.com/method/{0}?", methodName);
            NameValueCollection nameValueCollection = new NameValueCollection();
            foreach (KeyValuePair<string, object> keyValuePair in parameters)
            {
                nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value.ToString());
            }
            nameValueCollection.Add("access_token", _token);
            nameValueCollection.Add("group_id", _groupId.ToString());
            nameValueCollection.Add("v", "5.100");
            JToken jtoken;
            try
            {
                jtoken = JToken.Parse(this.Request(address, nameValueCollection).ToString());
            }
            catch
            {
                throw new RequestException();
            }
            if (jtoken["error"] == null)
            {
                return jtoken["response"];
            }
            if (Convert.ToInt32(jtoken["error"]["error_code"]) == 5)
            {
                Console.WriteLine(jtoken);
                throw new ApiException(jtoken["error"]["error_code"].ToString(), ExceptionCode.AccessToken);
            }
            throw new ApiException(jtoken["error"]["error_code"].ToString(), ExceptionCode.Other);
        }

        private void GetInfoLongPoll()
        {
            JToken jtoken = CallMethod("groups.getLongPollServer", new Dictionary<string, object>(1)
            {
                {
                    "lp_version",
                    "3"
                }
            });
            _ts = jtoken["ts"].ToString();
            _key = jtoken["key"].ToString();
            _server = jtoken["server"].ToString();
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
                    _key
                },
                {
                    "ts",
                    _ts
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
            return JToken.Parse(Request(string.Format("{0}", _server), parametrs));
        }

        public void Listen()
        {
            _isRunning = true;
            while (_isRunning)
            {
                JToken jtoken = RequestLongPoll();
                if (Convert.ToInt32(jtoken["failed"]) == 1)
                {
                    _ts = jtoken["ts"].ToString();
                }
                else if (Convert.ToInt32(jtoken["failed"]) == 2 || Convert.ToInt32(jtoken["failed"]) == 3)
                {
                    GetInfoLongPoll();
                }
                else
                {
                    _ts = jtoken["ts"].ToString();
                    foreach (JToken response in ((IEnumerable<JToken>)jtoken["updates"]))
                    {
                        UpdateEventArgs update = new UpdateEventArgs(response);
                        OnUpdate?.Invoke(update);
                    }
                }
            }
        }

        public void ListenAsync()
        {
            Task.Factory.StartNew(Listen);
        }

        public void StopListen()
        {
            _isRunning = false;
        }

        ~Api()
        {
            StopListen();
        }
    }
}
