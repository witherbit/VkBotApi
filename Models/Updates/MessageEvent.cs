using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkBotApi.Models
{
    public class MessageEvent : IUpdateEvent
	{
		public MessageEvent(JToken raw)
		{
			Raw = raw;
			raw = raw["object"];
			UserId = raw["user_id"].ToObject<long>();
			PeerId = raw["peer_id"].ToObject<long>();
			EventId = raw["event_id"].ToObject<string>();
			Position = int.Parse(raw["payload"]["button"].ToObject<string>());
            if(raw.ToString().Contains("conversation_message_id"))
				ConversationMessageId = raw["conversation_message_id"].ToObject<long>();
		}
		public long UserId { get; private set; }
		public long PeerId { get; private set; }
		public string EventId { get; private set; }
		public int Position { get; private set; }
		public long ConversationMessageId { get; private set; }
		public EventType Type { get => EventType.MessageEvent; }
		public JToken Raw { get; private set; }
	}
}
