using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VkBotApi.Core;
using VkBotApi.Models;
using VkBotApi.Models.Params;

namespace VkBotApi.Methods
{
    public sealed class Messages
    {
        internal Api _api;
        public async void AddChatUser(long chatId, long userId, int? visibleMessagesCount = null)
        {
            await Task.Run(()=>
            {
                var par = new Dictionary<string, object>
                {
                    { "chat_id", chatId },
                    { "user_id", userId }
                };
                if (visibleMessagesCount != null) par.Add("visible_messages_count", visibleMessagesCount);
                _api.CallMethod("messages.addChatUser", par);
            });
        }
        public async void CreateChat(List<ulong>userIds, string title)
        {
            await Task.Run(() =>
            {
                string user_ids = "";
                foreach (var userId in userIds)
                {
                    if (userId == userIds[userIds.Count - 1])
                        user_ids += userId.ToString();
                    else user_ids += userId.ToString() + ", ";
                }

                _api.CallMethod("messages.createChat", new Dictionary<string, object> 
                {
                    { "user_ids", user_ids },
                    { "title", title }
                });
            });
        }
        public JToken SendMessage(MessageParams message)
        {
            var parameters = new Dictionary<string, object>
            {
                { "peer_id", message.PeerId },
                { "message", message.Text },
                { "random_id", message.RandomId }
            };
            if (message.Lat != null)
                parameters.Add("lat", message.Lat);
            if (message.Long != null)
                parameters.Add("long", message.Long);
            if (message.ReplyTo != null)
                parameters.Add("reply_to", message.ReplyTo);
            if (message.StickerId != null)
                parameters.Add("sticker_id", message.StickerId);
            if (message.Keyboard != null)
                parameters.Add("keyboard", message.Keyboard.Build());

            return _api.CallMethod("messages.send", parameters);
        }
        public async Task<JToken> SendMessageAsync(MessageParams message)
        {
            return await Task.Run(()=>
            {
                return SendMessage(message);
            });
        }
    }
}
