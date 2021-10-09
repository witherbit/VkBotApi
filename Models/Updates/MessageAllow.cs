using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkBotApi.Models
{
    public class MessageAllow : IUpdateEvent
    {
		public MessageAllow(JToken raw)
		{
			Raw = raw;
			raw = raw["object"];
			UserId = raw["user_id"].ToObject<long>();
			Key = raw["key"].ToObject<string>();
		}
		public long UserId { get; private set; }
		public string Key { get; private set; }
		public EventType Type { get => EventType.MessageAllow; }
		public JToken Raw { get; private set; }
	}
}
