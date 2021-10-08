using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkBotApi.Models;

namespace VkBotApi.MessagesController
{
    public class UserManager : Dictionary<long,User>
    {
        public void Add(User user)
        {
            Add(user.Id, user);
        }
    }
}
