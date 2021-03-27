using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Models;

namespace VkBotApi.Core
{
    public static class Methods
    {
        public static JToken SendMessage(this Api api, Message message)
        {
            string peer_ids = "";
            foreach (var peerId in message.PeerIds)
            {
                if (peerId == message.PeerIds[message.PeerIds.Count - 1])
                    peer_ids += peerId.ToString();
                else peer_ids += peerId.ToString() + ", ";
            }
            var parameters = new Dictionary<string, object>
            {
                { "peer_ids", peer_ids },
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

            return api.CallMethod("messages.send", parameters);
        }
    }
}
