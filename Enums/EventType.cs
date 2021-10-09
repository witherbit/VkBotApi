using System;
using System.Collections.Generic;
using System.Text;

namespace VkBotApi.Enums
{
    public enum EventType
    {
        NewMessage,
        MessageReply,
        MessageEdit,
        MessageAllow,
        MessageDeny,
        MessageTypingState,
        MessageEvent,
        Raw
    }
}
