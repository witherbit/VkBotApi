using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkBotApi.Models
{
    public class MessageReply : IUpdateEvent
    {
		public MessageReply(JToken raw)
		{
			Raw = raw;
			raw = raw["object"];
			Date = raw["date"].ToObject<long>();
			UserId = raw["from_id"].ToObject<long>();
			Id = raw["id"].ToObject<long>();
			Out = raw["out"].ToObject<long>();
			PeerId = raw["peer_id"].ToObject<long>();
			Body = raw["text"].ToObject<string>();
			ConversationMessageId = raw["conversation_message_id"].ToObject<long>();
			ForwardMessages = raw["fwd_messages"];
			Important = raw["important"].ToObject<bool>();
			RandomId = raw["random_id"].ToObject<long>();
			Attachments = raw["attachments"];
			Hidden = raw["is_hidden"].ToObject<bool>();
			if (PeerId > 2000000000L)
			{
				FromChat = true;
				ChatId = PeerId - 2000000000L;
			}
			else
			{
				FromUser = true;
			}
		}
		public long Date { get; private set; }
		public long UserId { get; private set; }
		public long Id { get; private set; }
		public long Out { get; private set; }
		public long PeerId { get; private set; }
		public long ConversationMessageId { get; private set; }
		public long RandomId { get; private set; }
		public long ChatId { get; private set; }
		public string Body { get; private set; }
		public JToken ForwardMessages { get; private set; }
		public JToken Attachments { get; private set; }
		public bool Hidden { get; private set; }
		public bool Important { get; private set; }
		public bool FromUser { get; private set; }
		public bool FromChat { get; private set; }
		public EventType Type { get => EventType.MessageReply; }
		public JToken Raw { get; private set; }
	}
}
