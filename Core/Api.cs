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
using VkBotApi.Methods;

namespace VkBotApi.Core
{
    public sealed class Api : IDisposable
    {
        public static event EventHandler<ApiException> OnException;
        public int ApiVersion { get; set; } = 100;

        public string Token { get; private set; }
        
        public LongPoll LongPoll { get; private set; }

        public Api(string accessToken)
        {
            Token = accessToken;
            Init();
        }

        public Api(string accessToken, long groupId)
        {
            Token = accessToken;
            LongPoll = new LongPoll(this, groupId);
            Init();
        }
        #region Methods
        private void Init()
        {
            Messages = new Messages();
            Messages._api = this;
        }
        public Messages Messages { get; private set; }
        #endregion
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
            if (ApiVersion < 80 || ApiVersion > 110) SendException(this, new ApiException($"The api version cannot be lower than version 5.80 or higher than 5.110. The {methodName} method cannot be executed", ExceptionCode.Other));
            string address = string.Format("https://api.vk.com/method/{0}?", methodName);
            NameValueCollection nameValueCollection = new NameValueCollection();
            foreach (KeyValuePair<string, object> keyValuePair in parameters)
            {
                nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value.ToString());
            }
            nameValueCollection.Add("access_token", Token);
            if(LongPoll != null) nameValueCollection.Add("group_id", LongPoll.GroupId.ToString());
            nameValueCollection.Add("v", "5." + ApiVersion);
            JToken jtoken = null;
            try
            {
                jtoken = JToken.Parse(this.Request(address, nameValueCollection).ToString());
            }
            catch
            {
                SendException(this, new ApiException("Cannot send request", ExceptionCode.System));
            }
            if (jtoken["error"] == null)
            {
                return jtoken["response"];
            }
            if (Convert.ToInt32(jtoken["error"]["error_code"]) == 5)
            {
                SendException(this, new ApiException(jtoken["error"]["error_code"].ToString(), ExceptionCode.AccessToken));
            }
            SendException(this, new ApiException(jtoken["error"]["error_code"].ToString(), ExceptionCode.Other));
            return jtoken;
        }

        internal static void SendException(object sender, ApiException exception)
        {
            Task.Factory.StartNew(()=>OnException?.Invoke(sender, exception));
        }

        public void Dispose()
        {
            if(LongPoll != null)
                LongPoll.Dispose();
            GC.Collect();
        }

        public override string ToString()
        {
            return Token;
        }

        ~Api()
        {
            Dispose();
        }
    }
}
