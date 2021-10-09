using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkBotApi.Models
{
    public class MessageDeny : IUpdateEvent
    {
		public MessageDeny(JToken raw)
		{
			Raw = raw;
			raw = raw["object"];
			UserId = raw["user_id"].ToObject<long>();
		}
		public long UserId { get; private set; }
		public EventType Type { get => EventType.MessageDeny; }
		public JToken Raw { get; private set; }
	}
}
