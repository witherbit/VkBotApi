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
        /// <summary>
        /// VK API exception event. 
        /// </summary>
        public static event EventHandler<ApiException> OnException;
        /// <summary>
        /// VK API version (5.[value]).
        /// </summary>
        public int ApiVersion { get; set; } = 100;
        /// <summary>
        /// Access token.
        /// </summary>
        public string Token { get; private set; }
        /// <summary>
        /// LongPoll instance (Only for community initializer).
        /// </summary>
        public LongPoll LongPoll { get; private set; }
        /// <summary>
        /// Api initializer for user.
        /// </summary>
        /// <param name="accessToken">User access token.</param>
        public Api(string accessToken)
        {
            Token = accessToken;
            Init();
        }
        /// <summary>
        /// Api initializer for community.
        /// </summary>
        /// <param name="accessToken">Community access token.</param>
        /// <param name="groupId">Community id.</param>
        public Api(string accessToken, long groupId)
        {
            Token = accessToken;
            LongPoll = new LongPoll();
            LongPoll._api = this;
            LongPoll.GroupId = groupId;
            LongPoll.GetInfoLongPoll();
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
        /// <summary>
        /// Call VK API method.
        /// </summary>
        /// <param name="name">Method name.</param>
        /// <param name="parameters">Method parameters.</param>
        /// <returns></returns>
        public JToken CallMethod(string name, Dictionary<string, object> parameters)
        {
            if(ApiVersion < 80 || ApiVersion > 100)
            {
                SendException(this, new ApiException("The API version cannot be less than 5.80 or higher than 5.110", ExceptionCode.Other));
                return null;
            }
            string address = string.Format("https://api.vk.com/method/{0}?", name);
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
            GC.SuppressFinalize(this);
        }

        ~Api()
        {
            Dispose();
        }
    }
}
