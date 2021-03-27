using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.Models
{
    public class UpdateEventArgs
    {
		public UpdateEventArgs(JToken response)
		{
			Json = response;
			if (response["type"].ToObject<string>() == "message_new")
			{
				response = response["object"];
				Type = EventType.NewMessage;
				Date = response["date"].ToObject<long>();
				UserId = response["from_id"].ToObject<long>();
				Id = response["id"].ToObject<long>();
				Out = response["out"].ToObject<long>();
				PeerId = response["peer_id"].ToObject<long>();
				Body = response["text"].ToObject<string>();
				ConversationMessageId = response["conversation_message_id"].ToObject<long>();
				ForwardMessages = response["fwd_messages"];
				Important = response["important"].ToObject<bool>();
				RandomId = response["random_id"].ToObject<long>();
				Attachments = response["attachments"];
				Hidden = response["is_hidden"].ToObject<bool>();
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
			else Type = EventType.Other;
		}
		public EventType Type;
		public long Date;
		public long UserId;
		public long Id;
		public long Out;
		public long PeerId;
		public long ConversationMessageId;
		public long RandomId;
		public long ChatId;
		public string Body;
		public JToken ForwardMessages;
		public JToken Attachments;
		public bool Hidden;
		public bool Important;
		public bool FromUser;
		public bool FromChat;
		public JToken Json;
	}
}
