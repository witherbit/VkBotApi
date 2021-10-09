using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.Abstraction
{
    public interface IUpdateEvent
    {
        EventType Type { get; }
        JToken Raw { get; }
    }
}
