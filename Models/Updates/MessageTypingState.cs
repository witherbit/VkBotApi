using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkBotApi.Models
{
    public class MessageTypingState : IUpdateEvent
	{
		public MessageTypingState(JToken raw)
		{
			Raw = raw;
			raw = raw["object"];
			FromId = raw["from_id"].ToObject<long>(); 
			ToId = raw["to_id"].ToObject<long>();
			State = raw["state"].ToObject<string>();
		}
		public long FromId { get; private set; }
		public long ToId { get; private set; }
		public string State { get; private set; }
		public EventType Type { get => EventType.MessageTypingState; }
		public JToken Raw { get; private set; }
    }
}
