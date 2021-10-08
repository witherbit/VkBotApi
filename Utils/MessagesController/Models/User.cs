using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.MessagesController
{
    public class User
    {
        /// <summary>
        /// User Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Permission
        /// </summary>
        public Permission Permission { get; set; }
        /// <summary>
        /// Message step
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Message chapter
        /// </summary>
        public string Chapter { get; set; }
        public Dictionary<string, string> UserVars { get; set; }
    }
}
