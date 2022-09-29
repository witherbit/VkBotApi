using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Models;

namespace VkBotApi.Core
{
    public class MessageEvents
    {
        public int sex = 0;
        public event EventHandler<Message> OnMessage;
        public event EventHandler<MessageAllow> OnMessageAllow;
        public event EventHandler<MessageDeny> OnMessageDeny;
        public event EventHandler<MessageEdit> OnMessageEdit;
        public event EventHandler<MessageEvent> OnMessageEvent;
        public event EventHandler<MessageReply> OnMessageReply;
        public event EventHandler<MessageTypingState> OnMessageTypingState;
        internal void CallEvent(IUpdateEvent updateEvent, LongPoll longPoll)
        {
            if(updateEvent is Message)
            {
                OnMessage?.Invoke(longPoll, updateEvent as Message);
            }
            else if (updateEvent is MessageAllow)
            {
                OnMessageAllow?.Invoke(longPoll, updateEvent as MessageAllow);
            }
            else if (updateEvent is MessageDeny)
            {
                OnMessageDeny?.Invoke(longPoll, updateEvent as MessageDeny);
            }
            else if (updateEvent is MessageEdit)
            {
                OnMessageEdit?.Invoke(longPoll, updateEvent as MessageEdit);
            }
            else if (updateEvent is MessageEvent)
            {
                OnMessageEvent?.Invoke(longPoll, updateEvent as MessageEvent);
            }
            else if (updateEvent is MessageReply)
            {
                OnMessageReply?.Invoke(longPoll, updateEvent as MessageReply);
            }
            else if (updateEvent is MessageTypingState)
            {
                OnMessageTypingState?.Invoke(longPoll, updateEvent as MessageTypingState);
            }
        }
    }
}
