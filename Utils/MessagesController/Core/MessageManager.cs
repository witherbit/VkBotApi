using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Models;
using System.Linq;

namespace VkBotApi.MessagesController
{
    public class MessageManager : List<MessagePattern>
    {
        public Config Config = new Config();
        public UserManager UserManager = new UserManager();
        public MessagePattern Get(string request, User user)
        {
            var i = this.FirstOrDefault(r => r.Request.Contains(request) && r.Chapter == Config.DefaultSystemChapter);
            if (i != null) return i;
            return this.FirstOrDefault(r => r.Request.Contains(request) && r.Step == user.Step && r.Chapter == user.Chapter);
        }
        public MessagePattern Send(string request, long userId, Action<MessagePattern> sendAction, Action<string> argAction = null)
        {
            var message = Get(request, UserManager[userId]);
            if (message == null)
                message = new MessagePattern
                {
                    Chapter = Config.DefaultSystemChapter,
                    Step = 0,
                    PermissionType = UserManager[userId].Permission,
                    Request = new List<string> { request },
                    Response = Config.DefaultNotFoundMessage
                };
            if (message.PermissionType > UserManager[userId].Permission)
                message = new MessagePattern
                {
                    Request = new List<string> { request },
                    Chapter = Config.DefaultSystemChapter,
                    Step = 0,
                    PermissionType = UserManager[userId].Permission,
                    Response = Config.DefaultPermissionDeniedMessage
                };
            if (message.ReturnTo != null)
            {
                message.Response = message.ReturnTo.Response;
                message.Args = new string[] { $"step={message.ReturnTo.Step}", $"chapter={message.ReturnTo.Chapter}" };
            }
            sendAction?.Invoke(message);
            if(message.Args != null)
            {
                foreach(var arg in message.Args)
                {
                    if (arg.Contains("step="))
                    {
                        UserManager[userId].Step = int.Parse(arg.Replace("step=",""));
                    }
                    else if (arg.Contains("chapter="))
                    {
                        if(arg.Replace("chapter=", "") != Config.DefaultSystemChapter)
                            UserManager[userId].Chapter = arg.Replace("chapter=", "");
                    }
                    else
                        argAction?.Invoke(arg);
                }
            }
            return message;
        }
    }
}
