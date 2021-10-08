using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.MessagesController
{
    public class MessagePattern
    {
        /// <summary>
        /// Message requests (pattern)
        /// </summary>
        public List<string> Request { get; set; } = new List<string>();
        /// <summary>
        /// Message response (answer on pattern)
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// Permission type from the user
        /// </summary>
        public Permission PermissionType { get; set; } = Permission.User;
        /// <summary>
        /// Message step
        /// </summary>
        public int Step { get; set; } = 0;
        /// <summary>
        /// Message chapter
        /// </summary>
        public string Chapter { get; set; } = "default";
        /// <summary>
        /// Message args
        /// </summary>
        public string[] Args { get; set; }
        public MessagePattern ReturnTo { get; set; }
    }
}
