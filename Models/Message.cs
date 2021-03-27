using System;
using System.Collections.Generic;
using System.Text;

namespace VkBotApi.Models
{
    public class Message
    {
        public List<long> PeerIds = new List<long>();

        public string Text { get; set; }
        public int RandomId { get; set; }

        /// <summary>
        /// from -90 to 90
        /// </summary>
        public int? Lat { get; set; }
        /// <summary>
        /// from -180 to 180
        /// </summary>
        public int? Long { get; set; }

        public int? ReplyTo { get; set; }

        public int? StickerId { get; set; }
    }
}
