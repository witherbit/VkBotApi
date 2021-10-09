using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;
using VkBotApi.Abstraction;

namespace VkBotApi.Models
{
    public class UpdateRaw : IUpdateEvent
    {
        public UpdateRaw(JToken raw)
        {
            Raw = raw;
        }
        public EventType Type { get => EventType.Raw; }
        public JToken Raw { get; private set; }
    }
}
